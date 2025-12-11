<div align="center">

<h1>🎓📚 Contract Monthly Claim System (CMCS) 💻🐙</h1>

</div>

---

## 📑 Table of Contents

✨ 1. [**Introduction**](#-1-introduction)<br>
💻 2. [**Setting Up the Project Locally**](#-2-setting-up-the-project-locally)<br>
✅ 3. [**Features and Functionality**](#-3-features-and-functionality)<br>
🔑 4. [**Admin Login Credentials**](#-4-admin-login-credentials)<br>
🗺️ 5. [**Lecturer & Admin Navigation**](#️-5-lecturer--admin-navigation)<br>
🏗️ 6. [**Architecture**](#️-6-architecture)<br>
👥 7. [**Author and Contributions**](#-7-author-and-contributions)<br>
⚖️ 8. [**MIT License**](#️-8-mit-license)<br>
❓ 9. [**Frequently Asked Questions (FAQ)**](#-9-frequently-asked-questions-faq)<br>
📚 10. [**References**](#-10-references)<br>

---

## ✨ 1. Introduction

The **Contract Monthly Claim System (CMCS)** is a **robust, web-based application** developed using **.NET Core and C#** to revolutionize the **claim submission and approval process** for **Independent Contractor (IC) lecturers**. This system provides a streamlined, user-friendly, and efficient platform designed to handle complex calculations, documentation, and multi-stage approvals. The CMCS aims to eliminate manual inefficiencies by automating tasks, such as payment calculations and approval workflows, ensuring accuracy and accountability in administrative processes. It supports IC lecturers in submitting claims with supporting documents, while enabling Programme Coordinators and Academic Managers to easily verify and approve them. The final component includes an HR view to automate claim processing and lecturer data management, culminating in a comprehensive solution for modern claim management.

### Technical Features Highlights

- **Automated Calculations**: Features an auto-calculation function to compute final payments based on inputted hours worked and hourly rates.
- **Data Validation**: Integrates validation checks for accurate data entry and ensures the system provides consistent and reliable information.
- **Secure File Handling**: Allows lecturers to upload supporting documents, ensuring these files are securely stored and linked to the corresponding claim. File size and type restrictions (e.g., `.pdf`, `.docx`, `.xlsx`) are implemented.
- **Status Tracking**: Includes a real-time tracking system to transparently monitor the status of each claim as it moves through the approval process (e.g., 'Pending', 'Approved', 'Rejected').
- **Security & Authorization**: Employs ASP.NET Identity for robust user authentication and authorization across different user roles.
- **Reporting & Invoicing**: Features functionality to automatically generate invoices or reports for payment processing within the Admin view.
- **Advanced Data Retrieval Capabilities**: The system is equipped with comprehensive search and filtering functionalities, enabling swift and precise access to specific claim records and associated details.

---

## 💻 2. Setting Up the Project Locally

### Prerequisites

To successfully compile and run this project, you must have the following installed on your system:

- **Operating Systems**: Any OS compatible with the .NET 8.0 Runtime and the corresponding SDK. This generally includes modern versions of Windows (Windows 10/11), macOS 10.15+, or Linux distributions that support the .NET 8 framework.
- **IDE**: Compatible version of Microsoft Visual Studio 2019+ (or an equivalent IDE like VS Code with extensions, such as C# Dev Kit).
- **Version Control**: Git for cloning the repository.
- **Database**: SQL Server instance (either local or remote) is necessary to integrate with the main data store.
- **Frameworks**:
  - Target Framework: .NET 8.0 (net8.0)
  - Web Framework: ASP.NET Core 8.0
- **RAM**: Minimum 4GB
- **Disk Space**: Minimum 200MB free space
- **Dependencies**:
  - CMCS.csproj:
    - FluentValidation (Version: 11.11.0)
    - FluentValidation.AspNetCore (Version: 11.3.0)
    - itext (Version: 9.0.0)
    - itext7 (Version: 9.0.0)
    - iTextSharp (Version: 5.5.13.4)
    - Microsoft.AspNetCore.Identity.EntityFrameworkCore (Version: 8.0.8)
    - Microsoft.AspNetCore.Identity.UI (Version: 8.0.8)
    - Microsoft.EntityFrameworkCore (Version: 8.0.8)
    - Microsoft.EntityFrameworkCore.Design (Version: 8.0.8)
    - Microsoft.EntityFrameworkCore.Sqlite (Version: 8.0.8)
    - Microsoft.EntityFrameworkCore.SqlServer (Version: 8.0.8)
    - Microsoft.EntityFrameworkCore.Tools (Version: 8.0.8)
    - Microsoft.VisualStudio.Web.CodeGeneration.Design (Version: 8.0.4)
    - Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore (Version: 8.0.4)
    - toastr (Version: 2.1.1)
  - CMCSTest.csproj:
    - coverlet.collector (Version: 6.0.0)
    - FluentValidation (Version: 11.11.0)
    - FluentValidation.AspNetCore (Version: 11.3.0)
    - itext (Version: 9.0.0)
    - itext7 (Version: 9.0.0)
    - iTextSharp (Version: 5.5.13.4)
    - Microsoft.EntityFrameworkCore.InMemory (Version: 8.0.10)
    - Microsoft.NET.Test.Sdk (Version: 17.8.0)
    - Moq (Version: 4.20.72)
    - xunit (Version: 2.5.3)
    - xunit.runner.visualstudio (Version: 2.5.3)

### Project Configurations

#### `CMCS.csproj`

This configuration defines the project as an **ASP.NET Core web application** targeting the **latest framework version**.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="itext" Version="9.0.0" />
    <PackageReference Include="itext7" Version="9.0.0" />
    <PackageReference Include="iTextSharp" Version="5.5.13.4" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.8" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.8">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.8">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.4" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore" Version="8.0.4" />
    <PackageReference Include="toastr" Version="2.1.1" />
  </ItemGroup>

</Project>
```

#### `appsettings.json`

This configuration stores **connection strings**, **custom settings**, and **logging configuration**, which are loaded at runtime.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ApplicationDbContextConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;"
  }
}
```

#### `CMCSTest.csproj`

This configuration defines the project as an **XUnit** targeting the **latest framework version**.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>

    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="itext" Version="9.0.0" />
    <PackageReference Include="itext7" Version="9.0.0" />
    <PackageReference Include="iTextSharp" Version="5.5.13.4" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.10" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.5.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CMCS\CMCS.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
```

### Installation

Follow these steps to get the application running on your local machine.

#### 1. Clone the Repository

- Naviagte and click the green "**Code**" button at the top of this repository.
- Copy the URL under the "**HTTPS**" tab (`https://github.com/singhishkar108/Contract-Monthly-Claim-System-CMCS.git`).
- Navigate to the directory where you want to save the project (e.g., `cd Documents/Projects`).
- Open your preferred terminal or command prompt and use the following command to clone the project:

```bash
git clone https://github.com/singhishkar108/Contract-Monthly-Claim-System-CMCS.git
```

- This will create a new folder with the repository's name and download all the files and the entire history into it.
- Alternatively, you may download as a **ZIP file** or clone using **GitHub Desktop**.

#### 2. Open in Visual Studio (Recommended)

1.  Open **Visual Studio 2022**.
2.  Navigate to **File \> Open \> Project/Solution**.
3.  Browse to the cloned repository and select the **Solution file (.sln)** to load the project.
4.  Visual Studio will automatically perform a package restore (`dotnet restore`).

The application will launch. You should see a message in the console indicating the application is running. The browser should open automatically to the default URL.

#### 3. Configure Database Connection

The application connects to a **SQL database**. You must configure the connection string in the `appsettings.json` file. Create this file if it doesn't exist, and add the configuration using a placeholder structure.

> ⚠️ **Note**: If you are running locally, you will typically use a connection string pointing to a local SQL Server instance (e.g., using LocalDB).

#### 4. Apply Database Migrations

Use the **Entity Framework Core tools** to create the database schema based on the code's models. Run these commands from the main project directory (`CMCS`):

```bash
# Update the database to the latest migration
dotnet ef database update
```

This command will create the `CMCSDB` database (if it doesn't exist) and apply all necessary tables.

### Running

#### 1. Run in Visual Studio

1.  Select **Build \> Build Solution** (or press `F6`) to compile the project.
2.  Click the **Run** button (or press `F5`) to start the application with debugging, or `Ctrl+F5` to start without debugging.

#### 2. Run via Command Line (Alternative)

If you are using **Visual Studio Code** or prefer the CLI:

1.  Navigate to the project directory containing the `.csproj` file.
2.  Execute the following commands in sequence:

```bash
# Clean up any previous build files
dotnet clean

# Restore project dependencies
dotnet restore

# Build and run the application
dotnet run
```

#### 4. Access the Application

- The console output will indicate the local URL where the application is hosted (e.g., `https://localhost:7198`).
- Open your web browser and navigate to the displayed URL (e.g., `https://localhost:7198`). You should now see the **CMCS home page**.

---

## ✅ 3. Features and Functionality

### Core Claim Management Features

**1. Claim Submission Interface**

- **Data Entry**: Allows lecturers to input key claim data, including the total hours worked, the applicable hourly rate, and the claim period.
- **Auto-Calculation**: Automatically calculates the total claim amount in real-time based on the hours worked and rate provided, minimizing manual errors.

**2. Document Management**

- **Secure Uploads**: Functionality to upload essential supporting documents (e.g., teaching schedules, timesheets) to validate the claim.
- **File Restrictions**: Implements constraints on file size and type (e.g., PDF, DOCX, XLSX) to maintain data integrity and security.
- **Centralized Storage**: All linked documents are stored securely and associated directly with the corresponding claim record.

**3. Multi-Stage Approval Workflows**

- **Review Dashboards**: Dedicated views for each approver (Lecturer, Admin) to efficiently review all pending claims.

**4. Claim Status Tracking**

- **Transparency**: Provides real-time visibility into a claim's status (e.g., 'Pending', 'Approved', 'Rejected', 'Paid').

**5. HR and Payment Automation**

- **Data Consolidation**: Admin view provides a consolidated list of all approved claims awaiting payment processing.
- **Reporting**: Automatic generation of reports and summaries (using libraries like iTextSharp for PDF creation) of approved claims for external payroll system integration.

**6. Data Validation**

- **Fluent Validation**: Utilizes the FluentValidation library to implement complex, reliable, and user-friendly server-side and client-side validation rules, ensuring data accuracy before saving or processing.

### Security and User Management Features (ASP.NET Core Identity)

**1. User Registration and Authentication**

- **Secure Login**: Provides standardized, ready-to-use login, logout, and registration functionalities.
- **Password Management**: Handles secure storage of user credentials using industry-standard password hashing and includes features for password recovery/reset and strong password policies.

**2. Role-Based Access Control (RBAC)**

- **User Roles**: Manages distinct user roles: Lecturer (can submit and track claims), Admin (can review/approve claims).
- **Authorization**: Restricts access to specific pages, functions, or data based on the user's assigned role (e.g., only Admin can access the payment finalization dashboard).

---

## 🔑 4. Admin Login Credentials

**Email**: _admin@gmail.com_ <br>
**Password**: _Admin123!_ <br>

> ⚠️ **Note**: For demonstration purposes, the Admin credentials are given to sign in directly.
> This approach is **not recommended** for production environments. In a real deployment, several administrator accounts would be pre-seeded and roles securely assigned through controlled provisioning processes.

---

## 🗺️ 5. Lecturer & Admin Navigation

### Lecturer Role

**1. System Access and Initial Navigation**

- **Initial Access:** Users are directed to the Application Landing Page.
- **User Registration and Authentication:**
  - New users must navigate to the "Register" section to establish an account.
  - Following registration, users should proceed to the Login portal using their established credentials.
- **Administrator Access:**
  - Administrators must utilize their designated pre-approved credentials to log in.
  - Upon successful authentication, administrators are required to navigate to the "List of Claims" module for claim oversight.

**2. Claim Submission Workflow**

- **Claim Initiation:** Users must select the "Submit Claim" option from the main navigation menu.
- **Data Entry:** Complete all mandatory fields within the claim form, including, but not limited to, the applicable claim period, total hours worked, and the contracted rate per hour.
- **Final Submission:** Conclude the process by clicking the "Submit" button. The claim will then be forwarded to the approval queue.

### Administrator Role

**1. Administrative Claim Review**

- **Login Credentials**: Administartors may navigate to the "Login" page via the button in the header of the website, and enter in the admin login credentials.
- **Claim Oversight:** Access the "List of Claims" section to view a comprehensive ledger of all submitted claims.
- **Data Presentation:** All claim records are displayed in a structured, tabular format for efficient review.

**2. Claim Approval and Rejection**

- **Action Execution:** Within the "List of Claims" interface, select a specific claim record. Based on the verification outcome, the administrator must click either "Approve" or "Reject".
- **Post-Approval Routing:** Approved claims are automatically transitioned to the subsequent stage for payroll processing.

---

## 🏗️ 6. Architecture

### Application Structure (ASP.NET Core MVC)

The application code adheres to the **MVC pattern**, which ensures a clear separation of concerns, making the codebase maintainable, testable, and scalable.

- **Model**: This layer manages the application's data and business logic. It includes the Entity Framework Core data context, the entity classes (e.g., Product, Order), and the service classes responsible for interacting with the database and external Azure APIs.
- **View**: The user interface (UI) is rendered using Razor views. This layer is responsible solely for presenting the data to the client and capturing user input.
- **Controller**: Controllers act as the entry point for handling user requests. They receive input, coordinate the necessary actions by calling methods in the Model layer, and determine which View to return to the user.

---

## 👥 7. Author and Contributions

### Primary Developer:

- I, **_Ishkar Singh_**, am the sole developer and author of this project:
  Email (for feedback or concerns): **ishkar.singh.108@gmail.com**

### Reporting Issues:

- If you encounter any bugs, glitches, or unexpected behaviour, please open an Issue on the GitHub repository.
- Provide as much detail as possible, including:
  - Steps to reproduce the issue
  - Error messages (if any)
  - Screenshots or logs (if applicable)
  - Expected vs. actual behaviour
- Clear and descriptive reports help improve the project effectively.

### Proposing Enhancements:

- Suggestions for improvements or feature enhancements are encouraged.
- You may open a Discussion or submit an Issue describing the proposed change.
- All ideas will be reviewed and considered for future updates.

---

## ⚖️ 8. MIT License

**Copyright © 2025 Ishkar Singh**<br>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

## ❓ 9. Frequently Asked Questions (FAQ)

### 1. What functionalities does the CMCS provide, and how are they used?

1. **Claim Submission**

  - The **CMCS** enables lecturers to efficiently **prepare and submit their monthly claims** through an intuitive digital interface. Users are guided to input all required details, such as **hours worked**, **claim period**, and **hourly rates**. Once the form is completed, lecturers can submit their claims for administrative review and approval. This feature reduces manual paperwork and ensures accurate, timely submission of claims.

2. **Search and Filter Capabilities**

  - To assist users in quickly locating specific claims, the system offers **robust search and filtering tools**. Claims can be filtered by parameters such as **lecturer name**, **claim period**, **status**, or **date of submission**. These tools significantly improve efficiency, especially when managing a high volume of claims.

3. **User Management**

  - Administrators have access to **comprehensive user management controls**. Through the dedicated interface, admins can **add new users**, **edit existing user information**, **reset passwords**, or **deactivate accounts** when necessary. This functionality ensures that system access remains secure, up to date, and aligned with institutional requirements.

4. **Report Generation**

  - The CMCS supports the **generation of detailed reports** related to claims processing. Administrators can produce **summaries and detailed breakdowns** of submitted, approved, and rejected claims for any selected period. These reports assist the payroll department by providing accurate data for payment processing and offering valuable insights for auditing and record-keeping.

### 2. What steps should I take if the application becomes unresponsive?

If the application becomes unresponsive or freezes, try the following actions:

1. **Close and relaunch the application**:

   - This resolves most **temporary UI freezes** or non-critical memory handling issues.

2. **Check system resource usage**:

   - Open the **Task Manager** (Windows: `Ctrl + Shift + Esc`) and verify whether your system is experiencing high **CPU, memory, or disk usage**.
   - Look for any **background processes**—especially **.NET applications** or heavy programs—that may be interfering with the app’s performance.

3. **Confirm your system meets requirements**:

   - Ensure your device meets the **minimum runtime requirements** for the application, including the correct version of **.NET**.

4. **Restart your system if needed**:
   - If the application continues to freeze, performing a **full system restart** may help clear locked processes or resource conflicts.

- If the issue persists after these steps, please submit a **detailed report** via the **GitHub Issues page**.

### 3. How should I report a bug or unexpected application behaviour?

1. Thank you for helping improve the application. To report a bug:

   - **Open an Issue on GitHub**: Go to the repository’s **Issues tab** and **create a new issue**.

2. **Provide detailed information**:

- Include, where possible:

  - A **clear description** of the problem
  - **Step-by-step instructions** on how to reproduce the issue
  - **Expected behavior vs. actual behavior**
  - **Screenshots, console output, or error logs** (if applicable)
  - Your **system information** (operating system, .NET version, etc.)

3. **Use clear and descriptive titles**:
   - This helps **categorize and prioritize issues** efficiently.
   - The more detail provided, the easier it will be to diagnose and resolve the problem.

### 4. What should I do if the application fails to launch?

If the application fails to start, consider the following troubleshooting steps:

1. **Verify .NET Framework installation**:

   - Ensure that **.NET Framework 8** is installed on your system.
   - You may download the required runtime from **Microsoft’s official website** if necessary.

2. **Check the build integrity** (for source code users):

   - If running from the source code, ensure that the project **builds successfully** in your IDE with **no compilation errors**.
   - Confirm that all project dependencies and **NuGet packages** are properly **restored**.

3. **Ensure file integrity**:

   - Make sure no essential application files are **missing or corrupted**. If unsure, **re-download the application or clone the repository again**.

4. **Run as administrator** (if required):

   - Certain systems may restrict application execution. Right-click the executable and select **Run as administrator**.

5. **Verify `launchSettings.json` and `appsettings.json`**:
   - Check the `launchSettings.json` file and ensure the correct application URL and launch profile are configured. Review `appsettings.json` to confirm that all required configuration keys, especially those for connection strings and API endpoints, are present and hold valid values.

- If the application still does not launch, please **report the issue through GitHub** with system details and any error messages received.

---

## 📚 10. References

- **Anderson, Rick, and Learn Microsoft, 2024. “Get started with ASP.NET Core MVC.”** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0&tabs=visual-studio)_ [Accessed 25 September 2024].
- **Cherry, Kendra, and Verywell Mind, 2024. “The Color Blue: Meaning, Color Psychology, Effects.”** [online] _[verywellmind.com](https://www.verywellmind.com/the-color-psychology-of-blue-2795815)_ [Accessed 15 October 2024].
- **cho, jocelyn, and Medium, 2019. “Montserrat: A UI Case Study For A Typographical Specimen.”** [online] _[medium.com](https://medium.com/@jocelync12005/montserrat-a-ui-case-study-for-a-typographical-specimen-8eb169b1aa65)_ [Accessed 30 September 2024].
- **Google Fonts, n.d. “Download Success.”** [online] _[fonts.google.com](https://fonts.google.com/download/next-steps)_ [Accessed 10 November 2024].
- **Learn Microsoft, 2023. “Role-based authorization in ASP.NET Core.”** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0)_ [Accessed 12 September 2024].
- **Learn Microsoft, 2024. “Static files in ASP.NET Core | Microsoft Learn.”** [online] _[learn.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-8.0)_ [Accessed 5 November 2024].
- **SARS Home | South African Revenue Service, n.d. SARS Home | South African Revenue Service.** [online] _[sars.gov.za](https://www.sars.gov.za/)_ [Accessed 19 October 2024].
- **Troelsen, Andrew, and Phillip Japikse, 2021. Pro C# 9 with .NET 5: Foundational Principles and Practices in Programming.** N.p.: Apress.
- **YouTube and Digital TechJoint, 2022. “ASP.NET Identity - User Registration, Login and Log-out.”** [online] _[youtube.com](https://www.youtube.com/watch?v=ghzvSROMo_M)_ [Accessed 21 October 2024].
- **YouTube and Digital TechJoint, 2022. “ASP.NET MVC - How To Implement Role Based Authorization.”** [online] _[youtube.com](https://www.youtube.com/watch?v=qvsWwwq2ynE)_ [Accessed 1 November 2024].
