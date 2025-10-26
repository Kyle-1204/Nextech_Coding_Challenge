**Overview**



**How to Run**
1. Start the Backend
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApi
- dotnet run
- API runs on: https://localhost:5028
- Swagger UI: https://localhost:5028/swagger

2. Start Front-End (in separate terminal)
- cd C:\code\Nextech_Coding_Challenge\hacker-news-app
- ng serve --port 4200
- App runs on: http://localhost:4200

3. Backend Tests
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApi
- dotnet test

4. Frontend Tests
- cd C:\code\Nextech_Coding_Challenge\hacker-news-app
- ng test --watch=false --browsers=ChromeHeadless


**Challenges**


**Misc Notes**
- Frontend API URL is stored as a private readonly variable for simplicity; for security, this information would be secured in an environmental
**Final Thoughts**

