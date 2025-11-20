<h1 align="center">Business Verification Service</h1>

<h2>Description:</h2>

<p>This C# ASP.NET Core Web API is my contribution to an academic group project. It makes use of xUnit Test, NuGet Packages, Firebase tools, Docker, GitHub Actions and verification emails. The main focus of the project is a mobile application made with Flutter/Dart. It uses Firebase and a few services writen in C#, Java and Python as its backend. Further it uses Firestore as its database. I also did the integration of this service with the Flutter application and Firebase and I deployed it to Cloud Run. Visual Studio and Android Studio were used to develop my contributions.</p>

<p>This specific service focuses on comparing email and website address domains and then comparing those with business names to determine if the business requesting verification is a legit business. To do this the service receives a token from the Flutter application of the currently logged in user, with that token it authenticates that it is a valid user making the request from Firebase and retrives that user's relevant information from Firestore. It validates and normalizes the data and then performs the comparisons using the Nager.PublicSuffix and FuzzySharp NuGet Packages. After which it returns a response message to the Flutter application and writes the relevant information back into Firestore.</p>

<p>Additionally the service has the functionality to send verification emails when requested with a free Gmail account. It uses the MailKit NuGet Package to send emails using SMTP via Gmail. The service generates secure tokens using cryptography, with expiry dates that are included in the emails sent and uses Firestore to store and authenticate the tokens when users make use of the email verification process. When the verification links in these emails are clicked the service will authenticate it and wrtie the relevant information to Firestore.</p>

<p>This repository uses workflows in GitHub Actions to run any available xUnit Test projects when Pull Requests are made to the 'dev' and 'main' branches. It also builds and deploys a Docker image to Artifact Registry and is hosted on Cloud Run automatically when 'dev' is merged into 'main' on GitHub. Further the repository has rules so that branches can only be merged into 'dev' and 'main' via Pull Requests and I have made sure that no secrets or sensitive information is committed to the repository. The codebase and container images reference secret information via environment variables or local file paths that are outside of the online repository.</p>

<p>For the Flutter mobile application integration with this microservice I simply needed the URL that Cloud Run provided me with when hosting the Docker image and any endpoint paths in the microservice that need to be called. In the Dart code all the necessary imports were done and a function was written to call the relevant microservice endpoint with the capacity to receive any information that might be returned from the microservice. When the mobile application calls the microservice enpoint it also includes the currently logged in user's Firebase authentication token so that the microservice can authenticate the request and retrieve any data it might need from the user. I also did the styling of the input and output methods in the Dart files that are related to this microservice myself, with provisions for when the response might timeout and for other unexpected cases to make it user friendly and stylish. Further on the mobile application side of the project I also helped with the service methods on the admin page where admins can accept or reject business verification requests of businesses where the name only has somewhat of a correlation to the email verified domain name of the business.</p>

<p>Lastly, I deployed a Cloud Function to Firebase that simply listens for any user data updates and if that update affects a field that is used for business verificaiton, like the name, email or website address, the function will reset the business verification status accordingly. The business would then need to request verification of their name or their email again, depending on what information they updated.</p>

<p>All of this was done with free resources.</p>

<h2>Links:</h2>

* [Business verification demonstration video](https://youtu.be/z_E8GAKI6AU?si=XB-zAWfFpOJ-MT8E)
* [Group project repository](https://github.com/ChristiaanHuisman/EngagePoint_ITMDA_GroupS11)

<h2>Notes:</h2>

<p>I am truly sorry to anyone that is going through all the Pull Requests from the 'feature/EmailVerification' branch into the 'dev' branch and it's corresponding commits. I was struggling to get the testing and deploying of the Docker image through GitHub Actions working so that I can safely create and merge Pull Requests from 'dev' into 'main'. I wanted to ensure the CI/CD pipeline worked as expected before I developed the microservice further.</p>
