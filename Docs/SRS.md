# Software Requirements Specification (SRS): Order Management System (OMS)

## 1. Project Overview
ระบบจัดการคำสั่งซื้อ (Order Management System) พัฒนาขึ้นเพื่อจัดการกระบวนการสร้างและเรียกดูข้อมูลการสั่งซื้อ โดยเน้นความถูกต้องของการคำนวณราคาสินค้า ส่วนลด และภาษีมูลค่าเพิ่ม

## 2. Business Rules & Logic

### 2.1 Pricing Calculation
การคำนวณราคาสุทธิ (Net Amount) จะต้องเป็นไปตามขั้นตอนดังนี้:
1.  **Total Amount (Subtotal):** ผลรวมของราคาสินค้าทั้งหมดก่อนหักส่วนลด
2.  **Discount Amount:** คำนวณตามเงื่อนไข Coupon Code (ถ้ามี)
3.  **Taxable Amount:** `Total Amount - Discount Amount`
4.  **Tax Amount (VAT 7% Exclude):** `Taxable Amount * 0.07`
5.  **Net Amount:** `Taxable Amount + Tax Amount`

### 2.2 Coupon Rules
ระบบรองรับคูปองส่วนลด 2 ประเภท:
-   **SAVE10**: ส่วนลด 10% จากยอด Total Amount
-   **FLAT50**: ส่วนลดคงที่ 50 บาท (หาก Total Amount น้อยกว่า 50 บาท ส่วนลดจะเท่ากับ Total Amount)
-   *หมายเหตุ: สามารถใช้คูปองได้เพียง 1 โค้ดต่อ 1 คำสั่งซื้อเท่านั้น*

## 3. Functional Requirements

### 3.1 Order Creation
-   ผู้ใช้สามารถส่งรายการสินค้าและชื่อลูกค้าเพื่อสร้าง Order
-   ระบบต้องตรวจสอบความถูกต้องของรหัสส่วนลด
-   ระบบต้องคำนวณยอดเงินทั้งหมดโดยอัตโนมัติก่อนบันทึกลงฐานข้อมูล

### 3.2 Order Retrieval
-   ผู้ใช้สามารถดึงรายการคำสั่งซื้อทั้งหมดออกมาแสดงผลได้

## 4. Data Model (Entity: Order)

| Field Name | Data Type | Description |
| :--- | :--- | :--- |
| **Id** | UUID / String | Unique Identifier ของคำสั่งซื้อ |
| **CustomerName** | String | ชื่อลูกค้า |
| **TotalAmount** | Decimal | ยอดรวมราคาสินค้าทั้งหมด (ก่อนส่วนลดและภาษี) |
| **DiscountAmount**| Decimal | มูลค่าส่วนลดที่ได้รับ |
| **TaxAmount** | Decimal | ภาษีมูลค่าเพิ่ม (VAT 7%) |
| **NetAmount** | Decimal | ยอดชำระสุทธิ (Total - Discount + Tax) |
| **CreatedAt** | DateTime | วันที่และเวลาที่สร้างรายการ |

## 5. API Contracts

### 5.1 Create Order
**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "customerName": "John Doe",
  "items": [
    {
      "productName": "Laptop",
      "price": 30000,
      "quantity": 1
    }
  ],
  "couponCode": "SAVE10"
}
```

**Response (201 Created):**
```json
{
  "id": "ord_12345",
  "customerName": "John Doe",
  "totalAmount": 30000.00,
  "discountAmount": 3000.00,
  "taxAmount": 1890.00,
  "netAmount": 28890.00,
  "createdAt": "2023-10-27T10:00:00Z"
}
```

### 5.2 Get Orders
**Endpoint:** `GET /api/orders`

**Response (200 OK):**
```json
[
  {
    "id": "ord_12345",
    "customerName": "John Doe",
    "totalAmount": 30000.00,
    "discountAmount": 3000.00,
    "taxAmount": 1890.00,
    "netAmount": 28890.00,
    "createdAt": "2023-10-27T10:00:00Z"
  }
]
```

## 6. Non-Functional Requirements
-   **Accuracy:** การคำนวณทศนิยมต้องใช้ Precision สูง (แนะนำใช้ Decimal แทน Float)
-   **Performance:** การดึงข้อมูลคำสั่งซื้อควรมี Latency ต่ำกว่า 200ms
