// This is only for:
// Methods called by the admin page that has to do with the business verification process
// Thus, necessary imports and class definitions are assumed to be present in the actual file
// There is also a function for displaying a verified badge anywhare a verified business name is displayed, but that is not included here

// Returns a stream of all business users who are pending admin verification
Stream<List<UserModel>> getPendingBusinesses() {
    return _db
        .collection('users')
        .where('role', isEqualTo: 'business')
        .where('emailVerified', isEqualTo: true)
        .where('verificationStatus', isEqualTo: 'pendingAdmin')
        .orderBy('verificationRequestedAt', descending: true) // Oldest first
        .snapshots()
        .map((snapshot) =>
        snapshot.docs.map((doc) => UserModel.fromFirestore(doc)).toList()
    );
}

// Admin approves a business
Future<void> approveBusiness(String uid) async {
    await _db.collection('users').doc(uid).update({
        'verificationStatus': 'accepted'
    });
    await _db.collection('users').doc(uid).collection('businessVerification').doc(uid).update({
      'verificationStatus': 'accepted',
      'verificationStatusUpdatedAt': FieldValue.serverTimestamp()
    });
} 

// Admin rejects a business
Future<void> rejectBusiness(String uid) async {
    await _db.collection('users').doc(uid).update({
      'verificationStatus': 'rejected',

    });
    await _db.collection('users').doc(uid).collection('businessVerification').doc(uid).update({
      'verificationStatus': 'rejected',
      'verificationStatusUpdatedAt': FieldValue.serverTimestamp()
    });
}