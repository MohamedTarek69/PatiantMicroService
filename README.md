<h1 align="center">🧑‍⚕️ Patient Management MicroService</h1>

<p align="center">
  <b>ASP.NET Core Web API | Entity Framework Core | SQL Server | Microservices Architecture</b>
</p>

<p align="center">
  A dedicated microservice responsible for managing patient profiles, medical records, allergies, and healthcare information within the MedGuide Healthcare Platform.
</p>

---

## 🏗️ Project Overview

The Patient Management MicroService provides centralized patient data management and serves as the primary source of patient information across the MedGuide ecosystem.

The service enables:

- Patient Registration
- Patient Profile Management
- Medical Record Management
- Allergy Tracking
- Patient Health Information Storage
- Integration with Healthcare Services

---

## 🎯 Goals

- Maintain accurate patient information.
- Store and manage medical records securely.
- Track patient allergies and health conditions.
- Provide healthcare services with patient data.
- Support scalable healthcare operations.

---

## ✨ Main Features

| Feature | Description |
|----------|-------------|
| 👤 Patient Management | Create, update, retrieve, and delete patients |
| 📋 Patient Profiles | Manage personal and healthcare information |
| 🩺 Medical Records | Store and maintain patient medical history |
| 🌿 Allergy Management | Track allergies and medical sensitivities |
| 🔍 Patient Lookup | Search and retrieve patient details |
| 📄 Detailed Profiles | Access complete patient information |
| 🔗 Service Integration | Share patient data with healthcare services |

---

## 🧱 Architecture

The service follows a microservices-based architecture:

```text
Identity Service
       │
       ▼
Patient MicroService
       │
 ┌─────┴─────┐
 ▼           ▼
Medical     Allergies
Records
```

### Benefits

- Independent Deployment
- Scalability
- Data Isolation
- Maintainability
- Loose Coupling

---

## 🧰 Tech Stack

| Category | Technology |
|-----------|-------------|
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Architecture | Microservices |
| Documentation | Swagger |
| Mapping | AutoMapper |
| Design Patterns | Repository Pattern, Dependency Injection |
| Version Control | Git & GitHub |

---

# 👤 Patient Management

### Create Patient

```http
POST /Patiant/Create
```

Creates a new patient profile.

---

### Update Patient

```http
PUT /Patiant/Update
```

Updates patient information.

---

### Delete Patient

```http
DELETE /Patiant/Delete/{id}
```

Removes a patient profile.

---

### Get All Patients

```http
GET /Patiant/All
```

Retrieves all patients.

---

### Get Patient By Identity User Id

```http
GET /Patiant/DetailsByIdentityUserId/{identityUserId}
```

Retrieves patient information linked to an authenticated user.

---

# 📋 Patient Profile

### My Profile

```http
GET /Patiant/MyProfile
```

Returns basic patient profile information.

---

### My Details

```http
GET /Patiant/MyDetails
```

Returns complete patient details including medical information.

---

# 🩺 Medical Records

### Add Medical Data

```http
POST /Patiant/MyMedicalData
```

Adds patient medical information.

---

### Delete Medical Record

```http
DELETE /Patiant/MedicalRecord/{medicalRecordId}
```

Removes a medical record.

---

# 🌿 Allergy Management

### Add Allergy

Managed through:

```http
POST /Patiant/MyMedicalData
```

---

### Delete Allergy

```http
DELETE /Patiant/Allergy/{allergyId}
```

---

## 📦 Core Entities

### Patient

- Personal Information
- Gender
- Contact Information
- Profile Data

### Medical Record

- Medical Conditions
- Health History
- Medical Notes

### Allergy

- Allergy Name
- Severity
- Description

---

## 🗄️ Data Model

```text
Patient
 │
 ├── MedicalRecords
 │
 └── Allergies
```

### Relationships

- One Patient → Multiple Medical Records
- One Patient → Multiple Allergies

---

## 🔒 Security Features

- JWT Authentication
- User-Based Data Access
- Secure API Endpoints
- Role-Based Authorization

---

## 🧠 Key Concepts

- 🏥 Healthcare Data Management
- 👤 Patient Profiles
- 🩺 Medical Records
- 🌿 Allergy Tracking
- 📦 Repository Pattern
- 💉 Dependency Injection
- 🗃️ Entity Framework Core
- 🚀 RESTful API Design

---

## 🔄 Integration Within MedGuide

```text
                API Gateway
                      │
      ┌───────────────┼───────────────┐
      ▼               ▼               ▼
Identity       Patient Service   Doctor Service
Service              │
                     ▼
              AI ChatBot Service
```

The Patient Service acts as a central healthcare data provider for:

- Doctor & Clinic Service
- AI ChatBot Service
- Future Notification Services

---

## 🚀 Getting Started

### Clone Repository

```bash
git clone <repository-url>
```

### Configure Database

Update:

```json
appsettings.json
```

### Apply Migrations

```powershell
Update-Database
```

### Run Application

```bash
dotnet run
```

---

## 📌 Future Enhancements

- Medical File Uploads
- Patient Documents
- Insurance Management
- Health Analytics
- Docker Support
- Kubernetes Deployment

---

## 👨‍💻 Author

**Mohamed Tarek**

- GitHub: https://github.com/MohamedTarek69

---

<p align="center">
🧑‍⚕️ Centralized Patient Data Management for the MedGuide Healthcare Ecosystem.
</p>
