# Software Requirements Specification (SRS)
## Project: ระบบบริหารจัดการงานรับเหมาก่อสร้างและซ่อมบำรุงสาขา 7-11 (7-11 Field Service Management)

---

## 1. ภาพรวมของระบบ (Project Overview)
ระบบนี้เป็น Web Application ที่พัฒนาบน **Google Apps Script (GAS)** โดยใช้ **Google Sheets** เป็นฐานข้อมูล และ **Google Drive** สำหรับเก็บไฟล์รูปภาพ ออกแบบมาเพื่อให้การบริหารจัดการงานก่อสร้างและซ่อมบำรุงสาขา 7-11 มีความรวดเร็ว ตรวจสอบได้ และลดข้อผิดพลาดในการสื่อสารระหว่าง Admin และทีมช่างหน้างาน

---

## 2. ขอบเขตสิทธิ์ผู้ใช้งาน (User Roles & Permissions)

| บทบาท (Role) | สิทธิ์การเข้าถึงข้อมูล | หน้าที่หลัก |
| :--- | :--- | :--- |
| **Super Admin** | 100% (ทุกตาราง) | จัดการ User ทุกระดับ, แก้ไข Master Data, ดูรายงานสรุปค่าใช้จ่ายและภาพรวมทั้งหมด |
| **Admin** | ข้อมูลใบงาน และ Master Data | สร้างใบงาน, มอบหมายทีมช่าง, ตรวจรับงาน, จัดการข้อมูลสาขาและประเภทงาน |
| **Technician** | เฉพาะงานที่ได้รับมอบหมาย | ดูรายการงาน, เช็คอิน GPS, อัปโหลดรูปส่งงาน, รายงานปัญหาหน้างาน |

---

## 3. กฎทางธุรกิจ (Business Rules)

1.  **Authentication & Security:**
    *   เข้าสู่ระบบด้วย Phone (Username) และ Password/PIN
    *   ช่างสามารถเปลี่ยนรหัสผ่านเองได้ผ่านหน้า Profile
2.  **Job Lifecycle & Status Workflow:**
    *   **Pending (รอดำเนินการ):** งานถูกสร้างแต่ช่างยังไม่เข้าพื้นที่
    *   **In Progress (กำลังทำงาน):** สถานะเปลี่ยนอัตโนมัติเมื่อช่างกด "Check-in"
    *   **Submitted (ส่งงานแล้ว):** ช่างอัปโหลดรูปและบันทึกข้อมูลครบถ้วน กดยืนยันส่งงาน
    *   **Completed (ตรวจสอบเรียบร้อย):** Admin ตรวจสอบรูปภาพและข้อมูลแล้วกดยืนยันปิดงาน
3.  **GPS Validation:**
    *   ระบบต้องดึงพิกัด (Lat, Long) จาก HTML5 Geolocation API ทันทีที่กดปุ่ม Check-in
    *   Admin สามารถเปรียบเทียบพิกัดที่เช็คอินกับพิกัดจริงของสาขาได้
4.  **Data Integrity:**
    *   ใบงาน (Jobs) ต้องผูกกับ Master_Branch และ Master_JobType เสมอ
    *   การอัปโหลดรูปภาพต้องสร้าง Folder แยกตาม Job_ID ใน Google Drive เพื่อความระเบียบ

---

## 4. โครงสร้างข้อมูล (Data Model - Google Sheets)

### 4.1 ตารางผู้ใช้งาน (Users)
*   `User_ID`: String (PK)
*   `Name`: String (ชื่อ-นามสกุล/ชื่อทีม)
*   `Phone`: String (Unique - ใช้ Login)
*   `Password`: String
*   `Role`: String (Super Admin, Admin, Technician)
*   `Status`: String (Active, Inactive)

### 4.2 ตารางข้อมูลสาขา (Master_Branch)
*   `Branch_Code`: String (PK)
*   `Branch_Name`: String
*   `Province`: String
*   `District`: String
*   `Location`: String (Lat, Long)

### 4.3 ตารางประเภทงาน (Master_JobType)
*   `JobType_ID`: String (PK)
*   `JobType_Name`: String (e.g., ไฟฟ้า, ประปา, โครงสร้าง)
*   `Status`: String (Active, Inactive)

### 4.4 ตารางใบงานหลัก (Jobs)
*   `Job_ID`: String (PK)
*   `Branch_Code`: String (FK)
*   `JobType_ID`: String (FK)
*   `Assigned_Team_ID`: String (FK -> Users)
*   `Start_Date`: Date
*   `End_Date`: Date
*   `Status`: String
*   `Check_in_Time`: DateTime
*   `Check_in_LatLong`: String
*   `Finished_Time`: DateTime

### 4.5 ตารางข้อมูลประกอบ (Child Tables)
*   **Job_Photos:** `Photo_ID`, `Job_ID`, `Image_URL`, `Caption`, `Timestamp`
*   **Issue_Logs:** `Issue_ID`, `Job_ID`, `Issue_Detail`, `Reported_By`, `Timestamp`
*   **Billing_Expenses:** `Expense_ID`, `Job_ID`, `Item_Name`, `Amount`, `Receipt_URL`

---

## 5. ข้อกำหนดด้าน API (Server-Side Functions - GAS)

| Function Name | Description | Parameters | Return |
| :--- | :--- | :--- | :--- |
| `doGet(e)` | จุดเริ่มต้นเข้าเว็บแอป | - | `HtmlOutput` |
| `login(p, pwd)` | ตรวจสอบการเข้าสู่ระบบ | `phone, password` | `{success, user, role}` |
| `getJobs(role, id)` | ดึงรายการงานตามสิทธิ์ | `role, userId` | `Array<JobObjects>` |
| `saveCheckIn(jId, loc)` | บันทึกเช็คอิน GPS | `jobId, latlong` | `{success, time}` |
| `uploadPhoto(jId, b64, cap)`| อัปโหลดรูปเข้า Drive & Sheet | `jobId, base64, caption` | `{success, url}` |
| `submitJob(jId, iss)` | กดยืนยันส่งงาน | `jobId, issueDetail` | `{success}` |
| `createJob(data)` | Admin สร้างงานใหม่ | `jobDataObject` | `{success, jobId}` |
| `addBranch(data)` | Admin เพิ่มสาขาใหม่ | `branchObject` | `{success}` |

---

## 6. โครงสร้างหน้าจอ (UI Architecture)

1.  **หน้า Login:** ฟอร์มรับเบอร์โทรและรหัสผ่าน
2.  **Dashboard (Technician):**
    *   Tab 1: งานปัจจุบัน (Pending/In Progress) -> Card แสดงรหัสสาขา/ชื่อสาขา/ปุ่มจัดการ
    *   Tab 2: ประวัติงาน (Completed)
3.  **หน้าปฏิบัติงาน (Job Action):**
    *   ปุ่ม Check-in (ทำงานเมื่อยังไม่มีเวลาเช็คอิน)
    *   ส่วนการอัปโหลดรูป (กล้อง/ไฟล์)
    *   ช่องบันทึกปัญหา
    *   ปุ่มกดยืนยันส่งงาน
4.  **Dashboard (Admin):**
    *   ตารางสรุปงานพร้อม Filter (สถานะ, ทีมช่าง)
    *   ปุ่ม + สร้างใบงานใหม่ (เปิด Modal)
    *   ปุ่มดูรายละเอียดงาน (แสดงรูปพิกัดและข้อมูลปัญหา)
5.  **หน้า Master Data (Admin/Super Admin):**
    *   จัดการข้อมูลสาขา และผู้ใช้ระบบ
