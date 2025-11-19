// This is only for:
// Listening for updates to specific fields in Firestore and then updating other fields based on those updates

import { onDocumentUpdated } from "firebase-functions/v2/firestore";
import * as admin from "firebase-admin";
import * as logger from "firebase-functions/logger";

admin.initializeApp();
const db = admin.firestore();

// Function for resetting verification fields on certain user field updates
export const syncBusinessFields = onDocumentUpdated(
  {
    document: "users/{uid}",
    region: "africa-south1",
    timeoutSeconds: 60,
    minInstances: 0,
    maxInstances: 20,
  },
  async (event) => {
    const uid = event.params.uid;

    type UserDoc = {
      role?: string;
      email?: string;
      name?: string;
      website?: string;
      emailVerified?: boolean;
      verificationStatus?: string;
    };

    const beforeSnap = event.data?.before;
    const afterSnap = event.data?.after;

    const before = beforeSnap?.data() as UserDoc || {};
    const after = afterSnap?.data() as UserDoc || {};

    const role = after.role;
    const isBusiness = role === "business";

    const updatesToUser: Record<string, any> = {};
    const updatesToVerification: Record<string, any> = {};

    // Detect changes
    const hasEmailChanged = before.email !== after.email;
    const hasNameChanged = before.name !== after.name;
    const hasWebsiteChanged = before.website !== after.website;

    // All users: email changed - reset emailVerified
    if (hasEmailChanged) {
      updatesToUser.emailVerified = false;
    }

    // Business-specific logic
    if (isBusiness) {
      if (hasEmailChanged || hasNameChanged || hasWebsiteChanged) {
        updatesToUser.verificationStatus = "notStarted";

        const businessVerificationRef = db
          .collection("users")
          .doc(uid)
          .collection("businessVerification")
          .doc(uid);

        updatesToVerification.verificationStatus = "notStarted";
        updatesToVerification.verificationStatusUpdatedAt = admin.firestore.Timestamp.now();

        if (hasEmailChanged) {
          updatesToVerification.emailVerified = false;
        }

        // Apply updates in a batch
        const batch = db.batch();
        if (Object.keys(updatesToUser).length > 0) {
          batch.update(db.collection("users").doc(uid), updatesToUser);
        }
        batch.set(businessVerificationRef, updatesToVerification, { merge: true });

        logger.info("Applying business updates", { updatesToUser, updatesToVerification });
        return batch.commit();
      }
    }

    // Non-business users
    if (!isBusiness && hasEmailChanged && Object.keys(updatesToUser).length > 0) {
      logger.info("Applying non-business email update", updatesToUser);
      return db.collection("users").doc(uid).update(updatesToUser);
    }

    // No relevant changes - do nothing
    logger.info("No relevant changes detected, skipping update", { uid });
    return null;
  }
);