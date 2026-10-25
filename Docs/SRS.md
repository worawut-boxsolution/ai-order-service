# Software Requirements Specification (SRS)
## Project: Order Management System (OMS)

### 1. Introduction
เอกสารฉบับนี้ระบุถึงข้อกำหนดทางเทคนิคและกระบวนการทำงานของระบบจัดการคำสั่งซื้อ (Order Management System) เพื่อใช้เป็นแนวทางในการพัฒนา Backend API และการจัดการฐานข้อมูล

### 2. Functional Requirements
- ระบบสามารถสร้างคำสั่งซื้อใหม่ (Create Order) พร้อมคำนวณส่วนลดและภาษีได้
- ระบบสามารถเรียกดูรายการคำสั่งซื้อทั้งหมด (List Orders) ได้

### 3. Business Rules & Logic
การคำนวณราคาสินค้าต้องเป็นไปตามลำดับขั้นตอนดังนี้:

#### 3.1 การคำนวณส่วนลด (Discount Calculation)
รองรับคูปองส่วนลด 2 รูปแบบ:
1. **SAVE10**: ส่วนลด 10% จากยอดรวมสินค้า (TotalAmount)
2. **FLAT50**: ส่วนลดคงที่ 50 บาท
   *หมายเหตุ: หากไม่ระบุคูปอง หรือคูปองไม่ถูกต้อง ส่วนลดจะเป็น 0*

#### 3.2 การคำนวณภาษี (Tax Calculation)
- ใช้รูปแบบ **VAT 7% Exclude** (ภาษีมูลค่าเพิ่มแยกจากราคาสินค้า)
- คำนวณจากยอดหลังหักส่วนลด: `TaxAmount = (TotalAmount - DiscountAmount) * 0.07`

#### 3.3 การคำนวณยอดสุทธิ (Net Amount Calculation)
- `NetAmount = (TotalAmount - DiscountAmount) + TaxAmount`

---

### 4. Data Model
#### Entity: Order
| Field Name | Data Type | Description |
| :--- | :--- | :--- |
| **Id** | UUID / Integer | รหัสอ้างอิงคำสั่งซื้อ (Primary Key) |
| **CustomerName** | String | ชื่อลูกค้า |
| **TotalAmount** | Decimal | ยอดรวมราคาสินค้าก่อนหักส่วนลด |
| **DiscountAmount** | Decimal | จำนวนเงินส่วนลดที่ได้รับ |
| **TaxAmount** | Decimal | จำนวนภาษีมูลค่าเพิ่ม (7%) |
| **NetAmount** | Decimal | จำนวนเงินสุทธิที่ลูกค้าต้องชำระ |
| **CreatedAt** | DateTime | วันที่และเวลาที่สร้างรายการ |

---

### 5. API Endpoints Contracts

#### 5.1 Create Order
สร้างคำสั่งซื้อใหม่พร้อมคำนวณราคาสัตโนมัติ

- **URL:** `/api/orders`
- **Method:** `POST`
- **Request Body:**
```json
{
  "customerName": "string",
  "items": [
    {
      "productName": "string",
      "price": number,
      "quantity": number
    }
  ],
  "couponCode": "string" // Optional: "SAVE10", "FLAT50"
}
```

- **Response (201 Created):**
```json
{
  "id": "ORD-12345",
  "customerName": "John Doe",
  "totalAmount": 1000.00,
  "discountAmount": 100.00,
  "taxAmount": 63.00,
  "netAmount": 963.00,
  "createdAt": "2023-10-27T10:00:00Z"
}
```

#### 5.2 Get All Orders
เรียกดูรายการคำสั่งซื้อทั้งหมด

- **URL:** `/api/orders`
- **Method:** `GET`
- **Response (200 OK):**
```json
[
  {
    "id": "ORD-12345",
    "customerName": "John Doe",
    "totalAmount": 1000.00,
    "discountAmount": 100.00,
    "taxAmount": 63.00,
    "netAmount": 963.00,
    "createdAt": "2023-10-27T10:00:00Z"
  }
]
```

---

### 6. Error Handling
| Status Code | Description |
| :--- | :--- |
| 400 Bad Request | ข้อมูล Input ไม่ถูกต้อง หรือ TotalAmount ติดลบ |
| 500 Internal Server Error | เกิดข้อผิดพลาดที่ระบบ Server |
