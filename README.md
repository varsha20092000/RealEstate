<img width="586" height="676" alt="property_detail png" src="https://github.com/user-attachments/assets/9396a6b4-3636-4736-8aa0-4d145926393d" />
<img width="613" height="781" alt="Property png" src="https://github.com/user-attachments/assets/af5596de-8f2f-4dab-93ce-0478609d734b" />
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
<img width="705" height="861" alt="Screenshot 2026-03-20 172601" src="https://github.com/user-attachments/assets/03680117-74d2-450f-8b39-7a465d84ddd8" />
