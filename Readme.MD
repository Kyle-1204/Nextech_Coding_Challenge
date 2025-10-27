**Overview**
This program uses the Hacker News API to allow users to view the newest stories on their feed. To accomplish this, we implement a front-end UI using Angular that calls a C# .NET Core back-end restful API.


**How to Run**
All in separate terminals:
1. Start the Backend
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApi
- dotnet run
- API runs on: https://localhost:5028
- Swagger UI: https://localhost:5028/swagger

2. Start Front-End
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApp
- ng serve --port 4200
- App runs on: http://localhost:4200

3. Backend Tests
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApi
- dotnet test

4. Frontend Tests
- cd C:\code\Nextech_Coding_Challenge\HackerNewsApp
- ng test --watch=false --browsers=ChromeHeadless


**Requirements**
Using the Hacker News API, create a solution that allows users to view the newest stories from the feed.
 
Your solution must be developed using an Angular front-end app that calls a C# .NET Core back-end restful API.
 
The front-end UI should consist of:
- A list of the newest stories
- Each list item should include the title and a link to the news article. Watch out for stories that do not have hyperlinks!
- A search feature that allows users to find stories
- A paging mechanism. While we love reading, 200 stories on the page is a bit much.
- Automated tests for your code
 
- The back-end API should consist of:
- Usage of dependency injection, it's built-in so why not?
- Caching of the newest stories
- Automated tests for your code

While we would love seeing an Angular only solution to this problem, we really need to see your CSharpness.


**Architecture Structure**
- Backend - .NET Core 9.0 w/ C#
- Frontend - Angular 18.2.1
- Integration - RESTful API


**Project Structure**
Nextech_Coding_Challenge
- HackerNewsApi\ # .NetCore Backend
    - Controllers\
        - StoriesController.cs # API endpoints
    - Services\
        - HackerNewsService.cs # API integration service
        - IHackerNewsService.cs # Service Interface
    - Models\ # Data models
        - HackerNewsItem.cs # Story DTO Models
        - StoriesReponses # Response DTO Models
    - Program.cs # App configuration - *root of Dependency Injection configs
- HackerNewsAPI.Tests\
    - HackerNewsServiceTests.cs # Service unit tests
    - IntegrationTests.cs # API integration tests
    - StoriesControllerTests.cs # Story tests
- HackerNewsApp\ # Angular Frontend
    - src\app\
        - components\ # UI components - each component with web display, styling, unit tests, and functionality
            - story-list\ # displays list of story
            - search-bar\ # search component to filter stories by title
            - pagination\ # paginates stories
        - environments # env files for deployment
        - services\ # API services
            - hacker-news.services.ts 
        - models\ # TypeScript interfaces
            - hacker-news.model.ts
        - app.config.ts # App configuration
    - src\app\**\*.spec.ts # Units tests distributed throughout components


**Challenges**
- The Angular Project was initially created with Server-Side Rendering (SSR) enabled by default; however, this added unnecessary complexity, as it involved implementing additional files to compile the UI. As a result, I opted to remove SSR configuration to make it client-side only, which was a lengthy process with editing the configuration files (ie. angular.json). Besides that, the Angular front-end was very seamless to implement, with any roadblocks I encountered easily solved through online resources.
- In my current position, we predominantly utilize FastAPI to build and configure the backend API functionality. However, since this is exclusive to Python, I had to brush up my knowledge from my prior role to build the backend with the necessary requirements. Since I'm building this from scratch and you're prioritizing the C# Aspect of this, I made sure to be extremely diligent on documenting the process throughout the code and noting key aspects of it (particularly the dependency injections). This was completed by referencing past work in my PayFac position working with Admin Portal and Practice Plus and seeing how they can be applicable to this specific project
- Initially, I struggled to connect the front and back end components together, due to the browser preventing the two parts from interacting. Knowing how I used CORS add_middleware() to address this problem in my current project, I figured it was a similar process for C#; however, it took me some time to figure out the process of first creating the CORS policy manually and then applying it to the middleware.
- The front-end unit tests are self-explanatory; I simply just added .spec files to the components to cover all the functionality. However, the backend tests were more difficult for me, as I haven't experienced making unit tests for a .Net Core backend. Initially, I tried integrating the tests directly in the HackerNewsApi directory; however, this lead to issues with integrating the unit tests with the framework of the backend. To remedy this, a new directory was created specifically for the backend unit tests. From here, I tried to make the tests as comprehensive as possible cover the main functionalities of the backend.


**Final Thoughts**
This was an extremely fulfilling project to complete! While it was initially intimidating starting this project from scratch, the project flowed very smoothly. In particular, the Hacker News API was well-documented and easy to use, and my past experiences with Angular and .Net frameworks came into play. In the end, I view this project as an amalgamation of the skills and techniques I've learned throughout my time at Nextech. I am looking forward to seeing how my skills continue to develop during my time in this company, and I am happy to discuss more about the project in detail if interested. Thank you once again for this opportunity, and I look forward to speaking with you soon!

