


# 🏠 RealEstate — Full-Stack Property Management Platform

A comprehensive real estate web application built with ASP.NET Core 9 following Clean Architecture principles. The platform enables property buyers to browse, search, and inquire about properties, while agents can manage their listings, track analytics, and receive real-time notifications.

## ✨ Features

- 🔐 JWT Authentication with Admin, Agent, Buyer, and Seller roles
- 🔍 Elasticsearch-powered full-text property search with live results
- 🔔 SignalR real-time notifications for inquiries and bookings
- 💳 Razorpay payment gateway for booking deposits
- 🗺️ Interactive property maps using OpenStreetMap and Leaflet.js
- 📊 Agent analytics dashboard with Chart.js visualizations
- 📧 Transactional email notifications via Gmail SMTP
- 🏠 Property image upload and management
- 📅 Visit booking system with deposit payments
- 👑 Admin dashboard with platform statistics

## 🛠️ Tech Stack

### Backend
- ASP.NET Core 9 — Web API + MVC Framework
- Entity Framework Core 9 — ORM & Database Migrations
- SQL Server — Relational Database
- ASP.NET Identity — User Management & Authentication
- JWT Bearer Tokens — Role-based Authorization
- SignalR — Real-time WebSocket Notifications
- Elasticsearch — Full-text Search Engine
- Razorpay SDK — Payment Gateway Integration
- MailKit/MimeKit — SMTP Email Service

### Frontend
- ASP.NET Core MVC — Server-side Rendering
- Bootstrap 5.3 — Responsive UI Framework
- Font Awesome 6.4 — Icon Library
- JavaScript ES6+ — Dynamic UI & API calls
- Chart.js — Analytics Charts & Graphs
- Leaflet.js — Interactive Property Maps
- OpenStreetMap — Map Tile Provider
- SignalR JS Client — Real-time Notifications
- Razorpay Checkout JS — Payment Modal

## 🏗️ Architecture

Clean Architecture with 5 layers:
- **RealEstate.Domain** — Entities & Repository Interfaces
- **RealEstate.Application** — DTOs, Service Interfaces & Settings
- **RealEstate.Infrastructure** — DbContext, Repositories & Migrations
- **RealEstate.API** — Controllers, Services, Hubs
- **RealEstate.Web** — MVC Views & Frontend

## 🗄️ Database
9 tables: AppUser, Property, Agent, PropertyImage, Inquiry, Favorite, VisitBooking, Review, Payment

## 🔌 API
30+ REST endpoints across 9 controllers with Swagger documentation
<img width="1266" height="926" alt="Register png" src="https://github.com/user-attachments/assets/b8d2bb33-27d6-4c5c-9ff8-672105514564" />
<img width="705" height="861" alt="Screenshot 2026-03-20 172601" src="https://github.com/user-attachments/assets/03680117-74d2-450f-8b39-7a465d84ddd8" />
<img width="613" height="781" alt="Property png" src="https://github.com/user-attachments/assets/af5596de-8f2f-4dab-93ce-0478609d734b" />

<img width="586" height="676" alt="property_detail png" src="https://github.com/user-attachments/assets/d76d56a4-6439-4771-9a9c-4849ab58980a" />
<img width="620" height="333" alt="Admin_Dashboard png" src="https://github.com/user-attachments/assets/a3fa0482-9d99-4a47-a3c9-e825befda00a" />
<img width="618" height="506" alt="Buyer_inquiry png" src="https://github.com/user-attachments/assets/3afe0a6a-7f29-4e08-b156-79c36ad8848d" />
<img width="592" height="445" alt="Buyer account png" src="https://github.com/user-attachments/assets/3dffa1a1-ea36-44fd-b917-9e21d2c3ada6" />
<img width="727" height="906" alt="Agent_Dashboard png" src="https://github.com/user-attachments/assets/23923df2-1d81-4293-8a27-4c4e51f122ff" />
<img width="643" height="447" alt="admin_profile png" src="https://github.com/user-attachments/assets/1dfc479c-b0e2-48fe-99f4-367d7a5ebefd" />

