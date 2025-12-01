# B2B Marketplace Postman Collections - Workflow Documentation

This document provides comprehensive documentation for the Postman collections used to test the B2B Marketplace API endpoints. The collections cover four main user roles: Admin, Buyer, Seller, and Entrepreneur, with both detailed and simplified workflow tests incorporating all features from the workflow requirements document (agents-docs/raw/workflow.md).

## Collections Overview

### 1. B2B-Admin-Collection-Updated.postman_collection.json
**Purpose**: Complete admin workflow for platform management with comprehensive user, product, and shop management
**Key Features**:
- Admin authentication and authorization
- Dashboard and analytics management with comprehensive system statistics
- User management (list, search, ban/unban, password reset)
- Product and shop management (moderation, approval, removal)
- RFQ moderation and oversight
- System health monitoring
- Audit logging and reporting
- Report management (seller complaints, feedback reports)

### 2. B2B-Buyer-Collection-Updated.postman_collection.json
**Purpose**: Complete buyer workflow for discovering sellers, managing orders, and all required features
**Key Features**:
- Buyer registration and authentication
- Profile management
- Seller discovery and search
- Product browsing and management (latest products, product details)
- Cart management (add, update, remove items)
- Order management (place orders, track orders, view history)
- Address management (create, update, delete addresses)
- Payment methods management (add, update, delete payment methods)
- RFQ creation and management
- Review and rating system (for products and sellers)
- Complaint/report system (file reports against sellers)
- Notification management
- Transaction history viewing

### 3. B2B-Seller-Collection-Updated.postman_collection.json
**Purpose**: Complete seller workflow for managing business profile, inventory, orders, and all required features
**Key Features**:
- Seller registration and authentication
- Company profile management
- Online shop creation and management
- Product management (create, update, delete listings)
- Inventory management (update stock, view reports, low-stock alerts)
- Order management (receive, update status, send confirmations)
- RFQ response and quote management
- Statistics and dashboard (sales stats, product performance, customer analytics)
- Review management (respond to feedback)
- Notification management
- Business request handling (from entrepreneurs)

### 4. B2B-Entrepreneur-Collection.postman_collection.json
**Purpose**: Specialized collection for entrepreneur users with advanced business features
**Key Features**:
- Business profile management
- Business connection requests (send/receive partnership proposals)
- Business communication (specialized chat with sellers)
- Business order management (with special terms)
- Business reporting and analytics
- Advanced business features (network creation, insights)

### 5. B2B-Buyer-Collection.postman_collection.json
**Purpose**: Original simplified buyer workflow for backward compatibility
**Key Features**:
- Buyer registration and authentication
- Profile management
- Seller discovery and search
- RFQ creation and management
- Review and rating system
- Notification management

### 6. B2B-Seller-Collection.postman_collection.json
**Purpose**: Original simplified seller workflow for backward compatibility
**Key Features**:
- Seller registration and authentication
- Company profile management
- RFQ response and quote management

### 7. Buyer-Workflow-Tests.postman_collection.json
**Purpose**: Simplified buyer workflow tests
**Key Features**:
- Streamlined buyer registration and login
- Basic profile management
- Seller search and discovery
- RFQ creation and management
- Review creation
- Notification handling

### 8. Comprehensive-Admin-Workflow-Tests.postman_collection.json
**Purpose**: Complete admin workflow covering all features from agents-docs/raw functional requirements
**Key Features**:
- Admin authentication
- User management (lock/unlock accounts)
- Seller verification process
- Content moderation
- Service package management
- Category and certification management
- Analytics dashboard
- Audit logging

### 9. Seller-Workflow-Tests.postman_collection.json
**Purpose**: Simplified seller workflow tests
**Key Features**:
- Seller registration and authentication
- Company profile creation and management
- Certification management
- Product management
- RFQ response and quote submission
- Premium subscription
- Review management
- Notification handling

## Environment Variables

All collections use the following environment variables as appropriate for their role:

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `base_url` | API base URL | `http://localhost:5000` |
| `admin_token` | Admin JWT token | Generated during login |
| `buyer_token` | Buyer JWT token | Generated during login |
| `seller_token` | Seller JWT token | Generated during login |
| `cart_id` | Shopping cart identifier | Generated when creating cart |
| `order_id` | Order identifier | Generated when placing order |
| `product_id` | Product identifier | Generated when creating product |
| `address_id` | Address identifier | Generated when creating address |
| `payment_id` | Payment method identifier | Generated when adding payment method |
| `shop_id` | Shop identifier | Generated when creating shop |
| `review_id` | Review identifier | Generated when creating review |
| `feedback_id` | Feedback identifier | Generated when creating feedback |
| `request_id` | Business request identifier | Generated when creating business request |
| `message_id` | Message identifier | Generated when sending message |
| `notification_id` | Notification identifier | Generated by system |

## Common Test Patterns

### Authentication Flow
1. Register user with appropriate role
2. Login to obtain JWT token
3. Store token in environment variable
4. Use token in subsequent requests

### CRUD Operations
- **Create**: POST requests with JSON body
- **Read**: GET requests with optional query parameters
- **Update**: PUT requests with JSON body
- **Delete**: DELETE requests with path parameters

### Pagination
Most list endpoints support:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 50, max: 100)

### Error Handling
Tests expect appropriate HTTP status codes:
- `200`: Success
- `201`: Created
- `400`: Bad Request
- `401`: Unauthorized
- `404`: Not Found
- `500`: Internal Server Error

## Workflow Sequences

### Admin Workflow
1. **Authentication**: Login as admin
2. **Dashboard**: View statistics and analytics
3. **User Management**: List, search, ban/unban users, reset passwords
4. **Product Management**: Review and approve/remove products
5. **Shop Management**: Review and approve/suspend shops
6. **RFQ Moderation**: Moderate RFQ content
7. **Report Management**: Handle seller/user complaints and reports
8. **System Monitoring**: Check health and audit logs

### Buyer Workflow (Standard)
1. **Registration**: Create buyer account
2. **Authentication**: Login and get token
3. **Profile Setup**: Complete buyer profile
4. **Product Discovery**: Browse latest products, search for specific items
5. **Cart Management**: Add items to cart, update quantities, remove items
6. **Address Management**: Add and manage delivery addresses
7. **Payment Setup**: Add and manage payment methods
8. **Order Placement**: Place orders with selected products, address, and payment
9. **Order Tracking**: Track order status (Pending, Confirmed, Shipped, Delivered)
10. **Reviews**: Rate and review products and sellers
11. **Notifications**: Manage notification preferences

### Seller Workflow
1. **Registration**: Create seller account
2. **Authentication**: Login and get token
3. **Profile Setup**: Complete company profile
4. **Shop Creation**: Create and manage online shop
5. **Product Management**: Create and manage product listings
6. **Inventory Management**: Track and update stock levels
7. **Order Management**: Receive and process orders from buyers
8. **RFQ Response**: Browse and respond to RFQs
9. **Quote Management**: Submit and update quotes
10. **Statistics**: View sales and performance analytics
11. **Review Management**: Respond to customer feedback
12. **Notifications**: Manage notification preferences

### Entrepreneur Workflow (Business User)
1. **Registration**: Create entrepreneur account
2. **Authentication**: Login and get token
3. **Business Profile**: Complete business profile with company details
4. **Business Connection**: Send partnership requests to sellers
5. **Business Communication**: Engage in specialized business chats with sellers
6. **Business Orders**: Place orders with special business terms and requirements
7. **Analytics**: Access business purchase and supplier performance reports
8. **Network Management**: Create and manage business networks
9. **Standard Buyer Features**: All standard buyer functionality with enhanced business features

## Test Assertions

### Common Assertions
- **Status Code**: Verify HTTP response codes
- **Response Structure**: Check JSON structure and required fields
- **Data Types**: Validate data types (arrays, objects, strings, numbers)
- **Authentication**: Verify token-based access control
- **Pagination**: Check paginated response structure

### Role-Based Access
- **Admin**: Full platform access with elevated permissions
- **Buyer**: Limited to buyer-specific operations
- **Seller**: Limited to seller-specific operations
- **Entrepreneur**: Extended buyer functionality with business-specific features

## API Endpoints Coverage by Role

### Authentication Endpoints
- `POST /api/Auth/register` - User registration
- `POST /api/Auth/login` - User login
- `PUT /api/Auth/change-password` - Password change

### Profile Management Endpoints
- `GET /api/profile` - Get user profile
- `PUT /api/profile` - Update user profile
- `GET /api/profile/addresses` - Get user addresses
- `POST /api/profile/addresses` - Create/address
- `PUT /api/profile/addresses/{id}` - Update address
- `DELETE /api/profile/addresses/{id}` - Delete address

### Product Management Endpoints (Seller/Buyer/Admin)
- `GET /api/products` - Get all products
- `POST /api/products` - Create product listing
- `GET /api/products/{id}` - Get product details
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /api/products/latest` - Get latest products
- `POST /api/products/{id}/feedback` - Submit product feedback
- `GET /api/products/seller` - Get seller's products
- `PUT /api/products/{id}/inventory` - Update product inventory
- `GET /api/products/inventory/report` - Get inventory report
- `GET /api/products/inventory/low-stock` - Get low stock alert

### Cart Management Endpoints (Buyer)
- `POST /api/cart` - Create cart
- `POST /api/cart/{cartId}/items` - Add product to cart
- `GET /api/cart/{cartId}` - Get cart items
- `PUT /api/cart/{cartId}/items/{productId}` - Update cart item
- `DELETE /api/cart/{cartId}/items/{productId}` - Remove item from cart

### Order Management Endpoints (Buyer/Seller/Admin)
- `POST /api/orders` - Place order
- `GET /api/orders` - Get user orders
- `GET /api/orders/{id}` - Get order details
- `GET /api/orders/{id}/tracking` - Get order tracking status
- `PUT /api/orders/{id}/status` - Update order status
- `POST /api/orders/{id}/confirm` - Send order confirmation to buyer
- `GET /api/orders/received` - Get received orders (seller)
- `GET /api/orders/business-history` - Get business order history (entrepreneur)

### Shop Management Endpoints (Seller)
- `POST /api/shops` - Create online shop
- `GET /api/shops/{id}` - Get shop details
- `PUT /api/shops/{id}` - Update shop information

### Payment Methods Endpoints (Buyer)
- `POST /api/payment/methods` - Create payment method
- `GET /api/payment/methods` - Get payment methods
- `PUT /api/payment/methods/{id}/default` - Set default payment method
- `DELETE /api/payment/methods/{id}` - Delete payment method

### Search & Discovery Endpoints (Buyer/Seller)
- `GET /api/search/sellers` - Search sellers with filters
- `GET /api/public/sellers/{id}` - Get seller details
- `GET /api/products?sellerId={id}` - Get seller products

### RFQ Management Endpoints (Buyer/Seller)
- `POST /api/rfq` - Create RFQ
- `GET /api/rfq/buyer` - Get buyer's RFQs
- `GET /api/rfq/{id}` - Get RFQ details
- `PUT /api/rfq/{id}/status` - Update RFQ status
- `GET /api/rfqs` - Get available RFQs (sellers)
- `POST /api/quotes` - Submit quote
- `GET /api/quotes/my-quotes` - Get seller quotes
- `PUT /api/quotes/{id}` - Update quote

### Review & Rating Endpoints (Buyer/Seller)
- `POST /api/reviews` - Create review for seller
- `GET /api/reviews/seller/{id}` - Get seller reviews
- `GET /api/reviews/my-reviews` - Get user's reviews
- `POST /api/reviews/{id}/reply` - Reply to review (seller)
- `GET /api/reviews` - Get all reviews (admin)

### Communication Endpoints (Buyer/Seller)
- `POST /api/messages` - Send message
- `GET /api/messages/{recipientId}` - Get chat messages
- `GET /api/messages/business-chats` - Get business chats (entrepreneur)

### Business Requests Endpoints (Entrepreneur/Seller)
- `POST /api/requests/business` - Send business request
- `GET /api/requests/business-sent` - Get sent business requests
- `GET /api/requests/business-received` - Get received business requests
- `PUT /api/requests/business/{id}/status` - Update business request status

### Report Management Endpoints (Buyer/Admin)
- `POST /api/reports` - Report seller
- `GET /api/Admin/reports` - Get all reports (admin)
- `GET /api/Admin/reports/feedback` - Get feedback reports (admin)
- `PUT /api/Admin/reports/{id}/review` - Review report (admin)

### Admin Operations Endpoints (Admin)
- `GET /api/Admin/users` - List all users
- `GET /api/Admin/users/search` - Search users
- `GET /api/Admin/users/{id}` - Get user details
- `PUT /api/Admin/users/{id}/status` - Update user status
- `PUT /api/Admin/users/{id}/reset-password` - Reset user password
- `GET /api/Admin/products` - List all products
- `GET /api/Admin/products/{id}` - Get product details
- `PUT /api/Admin/products/{id}/status` - Update product status
- `GET /api/Admin/shops` - List all shops
- `GET /api/Admin/shops/{id}` - Get shop details
- `PUT /api/Admin/shops/{id}/status` - Update shop status
- `GET /api/Admin/rfqs` - List all RFQs
- `PUT /api/Admin/rfqs/{id}/moderate` - Moderate RFQ
- `GET /api/Admin/dashboard/stats` - Get dashboard stats
- `GET /api/Admin/analytics` - Get analytics overview
- `GET /api/Admin/system/health` - Get system health
- `GET /api/Admin/audit-logs` - Get audit logs
- `PUT /api/Admin/system/settings` - Update system settings

### Statistics/Dashboard Endpoints (Seller)
- `GET /api/dashboard/sales-stats` - Get sales statistics
- `GET /api/dashboard/product-performance` - Get product performance
- `GET /api/dashboard/customer-analytics` - Get customer analytics

### Analytics/Reports Endpoints (Entrepreneur)
- `GET /api/analytics/business-purchases` - Get business purchase analytics
- `GET /api/reports/supplier-performance` - Get supplier performance report

### Business Network Endpoints (Entrepreneur)
- `POST /api/business/networks` - Create business network
- `GET /api/business/networks/insights` - Get business network insights

### Notifications Endpoints (Buyer/Seller/Admin)
- `GET /api/notifications` - Get notifications
- `PUT /api/notifications/preferences` - Update preferences
- `PUT /api/notifications/{id}/read` - Mark as read

### Transaction History Endpoints (Buyer)
- `GET /api/transactions/history` - Get transaction history

## Running the Tests

### Prerequisites
1. Newman CLI installed
2. B2B Marketplace API running on `http://localhost:5000`
3. Database properly configured

### Execution Commands
```bash
# Run updated collections
newman run docs/postman-collections/B2B-Admin-Collection-Updated.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Admin-Collection-Updated-results.json

newman run docs/postman-collections/B2B-Buyer-Collection-Updated.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Buyer-Collection-Updated-results.json

newman run docs/postman-collections/B2B-Seller-Collection-Updated.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Seller-Collection-Updated-results.json

newman run docs/postman-collections/B2B-Entrepreneur-Collection.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Entrepreneur-Collection-results.json

# Run original collections
newman run docs/postman-collections/B2B-Admin-Collection.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Admin-Collection-results.json

newman run docs/postman-collections/B2B-Buyer-Collection.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Buyer-Collection-results.json

newman run docs/postman-collections/B2B-Seller-Collection.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/B2B-Seller-Collection-results.json

# Run workflow tests
newman run docs/postman-collections/Buyer-Workflow-Tests.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/Buyer-Workflow-Tests-results.json

newman run docs/postman-collections/Comprehensive-Admin-Workflow-Tests.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/Comprehensive-Admin-Workflow-Tests-results.json

newman run docs/postman-collections/Seller-Workflow-Tests.postman_collection.json --reporters cli,json --reporter-json-export docs/test-results/Seller-Workflow-Tests-results.json
```

### Test Results
Results are saved as JSON files in `docs/test-results/` directory with detailed information about:
- Test execution status
- Response times
- Assertion failures
- Request/response details

## Troubleshooting

### Common Issues
1. **401 Unauthorized**: Check token validity and expiration
2. **404 Not Found**: Verify endpoint URLs and parameters
3. **500 Internal Server Error**: Check API logs for server-side errors
4. **400 Bad Request**: Validate request body structure
5. **Missing Environment Variables**: Ensure all required environment variables are set correctly

### Environment Setup
- Ensure API is running on the correct port (5000)
- Verify database connectivity
- Check JWT token configuration
- Validate CORS settings for cross-origin requests

## Integration with CI/CD

These collections can be integrated into CI/CD pipelines for automated API testing with full workflow coverage including all requirements from agents-docs/raw/workflow.md. The updated collections ensure comprehensive testing of all core system functionality including cart management, order tracking, payment methods, address management, inventory tracking, business-specific features, and administrative controls as specified in the workflow requirements document.

```yaml
# Example GitHub Actions workflow
- name: Run API Tests
  run: |
    npm install -g newman
    newman run docs/postman-collections/B2B-Buyer-Collection-Updated.postman_collection.json --reporters cli,junit --reporter-junit-export test-results.xml
```

## Maintenance

### Adding New Tests
1. Create new request in appropriate collection
2. Add test scripts for assertions
3. Update environment variables as needed
4. Document new endpoints in this file

### Updating Existing Tests
1. Modify request parameters/body as needed
2. Update test assertions to match API changes
3. Verify backward compatibility
4. Update documentation

This documentation provides a comprehensive guide for using and maintaining the Postman collections for the B2B Marketplace API testing, fully aligned with the workflow requirements from agents-docs/raw/workflow.md. The updated collections ensure complete coverage of all use cases for standard users, entrepreneurs, sellers, and administrators.
