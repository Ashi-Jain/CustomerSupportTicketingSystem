# CustomerSupportTicketingSystem

# Project Overview
The Customer Support Ticketing System is a full-stack web application designed to manage
customer support requests efficiently.
Customers can raise tickets for issues, and staff members can respond, track progress, and
resolve them.
The system is divided into independent microservices — AuthService, CustomerService,
StaffService — all accessed via a central Ocelot API Gateway.
The React.js front end interacts with the backend through REST APIs using Axios.

# System Architecture
React Frontend (Port 3000)
↓
Ocelot API Gateway (Port 7007)
↓
┌──────────────────────────────────────┐
│ Microservices Layer
│ AuthService (7001)
│ CustomerService (7003)
│ StaffService (7005)
└──────────────────────────────────────┘
↓
SQL Server (CTS_MainDb)

# Front-End (React.js)
Technology Stack:
- Framework: React.js (CRA)
- UI Library: Tailwind CSS
- State Management: React Context API
- API Handling: Axios
- Routing: React Router DOM
- Authentication: JWT stored in localStorage
- IDE: Visual Studio Code
# Key Features:
- Login & Signup for both Customer and Staff
- Role-based dashboards
- Protected routes using JWT validation
- Ticket creation, update, and viewing
- Staff ticket management and commenting
- Axios Interceptor for Authorization header

# Folder Structure:
customer-support-app/
│
├── src/
│ ├── components/
│ ├── context/
│ ├── services/
│ ├── pages/
│ ├── App.js
│ └── main.js
│
├── package.json

# Overview
The front end of the Customer Support Ticketing System is developed using React.js,
providing a responsive, user-friendly interface for both customers and staff members.
It handles user interactions, manages authentication, and consumes REST APIs from the
ASP.NET Core backend through the Ocelot API Gateway.
Objectives
- To create a responsive and interactive user interface using React.js.
- To provide separate dashboards for Customer and Staff roles.
- To integrate with the backend via RESTful APIs for ticket and comment management.
- To implement JWT-based authentication and role-based access control.
- To maintain clean component structure and smooth navigation.

# Key Components
- Login & Signup: Handles authentication for both Customer and Staff roles.
- AuthContext: Manages global authentication state and JWT token storage.
- Customer Dashboard: Allows customers to create, view, update, delete, and comment on
tickets.
- Staff Dashboard: Enables staff to manage tickets and update status.
- TicketForm and CommentSection: Manage ticket and comment operations.
State Management
The app uses React Context API for global authentication state, providing login/logout
methods, token management, and route protection.

# Routing
Implemented with React Router DOM for navigation between pages:
- /login → Login
- /signup → Registration
- /customer/dashboard → Customer Dashboard
- /staff/dashboard → Staff Dashboard
API Integration
All API calls are made via Axios through Ocelot API Gateway (https://localhost:7007).
Authorization header is automatically attached using JWT token.
User Experience
- Customers: Can register, log in, manage tickets, and interact via comments.
- Staff: Can view, manage, and respond to tickets.

# Backend (ASP.NET Core Web API)

# Microservices:
- AuthService (7001) – Handles user login and registration
- CustomerService (7003) – Ticket creation and management
- StaffService (7005) – Staff ticket monitoring
- ApiGateway (7007) – Acts as a single unified entry point for routing requests to respective
microservices.
Authentication APIs:
- POST /auth/customer/register – Register new customer
- POST /auth/staff/register – Register new staff
- POST /auth/customer/login – Customer login
- POST /auth/staff/login – Staff login
Customer APIs :
(via Gateway → CustomerService)
Authorization: Bearer <customer_token>
POST /customer/tickets – Create a new ticket
GET /customer/tickets/mine – Get all tickets created by the current customer
GET /customer/tickets/{id} – Get ticket by ID
PUT /customer/tickets/{id} – Update ticket title or description
DELETE /customer/tickets/{id} – Delete a ticket created by the customer
POST /customer/tickets/{ticketId}/comments – Add comment to a specific ticket
GET /customer/tickets/{ticketId}/comments – View all comments for a ticket

Staff APIs:
(via Gateway → StaffService)
Authorization: Bearer <staff_token>
GET /staff/tickets – View all tickets created by customers
GET /staff/tickets/{id} – Get ticket details by ID
PUT /staff/tickets/{id}/status – Update ticket status (Open, InProgress, Closed)
POST /staff/tickets/{ticketId}/comments – Add a comment to a ticket
GET /staff/tickets/{ticketId}/comments – View all comments for a ticket
PUT /staff/tickets/{id}/status – Close the ticket by setting status to Closed
Base URL
https://localhost:7007

# Database Design
Tables:
1. Users – Stores user information (UserId, Name, Email, PasswordHash, Role)
2. Tickets – Stores ticket details and status
3. Comments – Stores comments with author role (Customer/Staff)

# Workflow Summary
1. Customer registers and logs in.
2. Customer creates a ticket.
3. Staff logs in and views tickets.
4. Staff updates ticket status and comments.
5. Customer can reply or close the ticket.
6. Status transitions: Open → InProgress → Closed.
8. Local Setup Instructions

# Front End Setup:
1. Install Node.Js and Visual studio Code
2. npm install
3. npm start

# Backend Setup:
1. Clone or open the CustomerTicketingSystem solution in Visual Studio.
2. Ensure SQL Server is running locally and update connection strings in all projects’
appsettings.json.
3. Build the solution to restore all NuGet packages.
4. Open the Package Manager Console in Visual Studio.
▪ Set “AuthService” as the Default Project → run:
Add-Migration InitAuth
Update-Database
▪ Set “CustomerService” as the Default Project → run:
Add-Migration InitCustomer
Update-Database
▪ Set “StaffService” as the Default Project → no migration needed if shared DB is
already created.
5. Set “ApiGateway” as the startup project (or multiple startup projects for all
services).
6. Run the solution.
7. Verify that the following URLs open in browser:
• AuthService → https://localhost:7001/swagger
• CustomerService → https://localhost:7003/swagger
• StaffService → https://localhost:7005/swagger
• ApiGateway → https://localhost:7007/swagger

# Testing
Use Swagger (https://localhost:7007/swagger) or React UI to test APIs.
JWT token from login is stored and sent automatically for protected routes.
10. Expected Workflow Outcome
1. Customer can create, view, update, delete, and comment on their own tickets.
2. Staff can view all tickets, update status, and comment.
3. Comments show who made them (Customer or Staff).
4. All requests return HTTP 200 OK.
5. Tokens are required for every protected API.
