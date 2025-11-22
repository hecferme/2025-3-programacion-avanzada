# BookCopy Statistics Feature

This document describes the new BookCopy statistics feature with real-time SignalR updates.

## Components

### 1. SignalR Hub
- **File**: `Hubs/BookCopiesStatsHub.cs`
- **Purpose**: Broadcasts real-time statistics updates to all connected clients
- **Endpoint**: `/bookCopiesStatsHub`

### 2. Repository
- **Interface**: `Repositories/RepositoryInterfaces.cs` → `IBookCopyRepository`
- **Implementation**: `Repositories/EfBookCopyRepository.cs`
- **Methods**:
  - `GetAllAsync()` - Returns all book copies
  - `GetByIsLostAsync(bool isLost)` - Filters book copies by lost status
  - `GetTotalCountAsync()` - Returns total count of book copies
  - `GetLostCountAsync()` - Returns count of lost book copies
  - `InsertAsync()`, `UpdateAsync()`, `DeleteAsync()` - CRUD operations that trigger SignalR broadcasts

### 3. API Controller
- **File**: `Controllers/Api/BookCopiesController.cs`
- **Endpoint**: `GET /api/BookCopies`
- **Header Support**: 
  - Without `isLost` header → Returns all book copies
  - With `isLost: 1` header → Returns only lost book copies
  - With `isLost: 0` header → Returns only available book copies
- **Additional Endpoints**:
  - `GET /api/BookCopies/{id}` - Get single book copy
  - `POST /api/BookCopies` - Create new book copy
  - `PUT /api/BookCopies/{id}` - Update book copy
  - `DELETE /api/BookCopies/{id}` - Delete book copy
  - `GET /api/BookCopies/stats` - Get statistics (JSON)

### 4. MVC Controller
- **File**: `Controllers/BookCopiesStatsController.cs`
- **Route**: `/BookCopiesStats/Index`
- **Purpose**: Displays the statistics page with initial data

### 5. View
- **File**: `Views/BookCopiesStats/Index.cshtml`
- **Features**:
  - Displays three statistics cards:
    - Total Book Copies (blue)
    - Lost Books (red)
    - Available Books (green)
  - Real-time updates via SignalR
  - Connection status indicator
  - Visual feedback when statistics change

## Usage

### Accessing the Statistics Page
Navigate to: `https://localhost:{port}/BookCopiesStats/Index`

### Using the API

#### Get all book copies:
```bash
curl -X GET https://localhost:{port}/api/BookCopies
```

#### Get only lost book copies:
```bash
curl -X GET https://localhost:{port}/api/BookCopies -H "isLost: 1"
```

#### Get only available book copies:
```bash
curl -X GET https://localhost:{port}/api/BookCopies -H "isLost: 0"
```

#### Get statistics:
```bash
curl -X GET https://localhost:{port}/api/BookCopies/stats
```

#### Create a new book copy:
```bash
curl -X POST https://localhost:{port}/api/BookCopies \
  -H "Content-Type: application/json" \
  -d '{
    "bookId": 1,
    "isbn": "978-1234567890",
    "serial": "COPY-001",
    "language": "English",
    "provider": "Publisher ABC",
    "isLost": false
  }'
```

#### Update a book copy (mark as lost):
```bash
curl -X PUT https://localhost:{port}/api/BookCopies/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "bookId": 1,
    "isbn": "978-1234567890",
    "serial": "COPY-001",
    "language": "English",
    "provider": "Publisher ABC",
    "isLost": true
  }'
```

## Real-Time Updates

Whenever a book copy is:
- **Created** (via API or repository)
- **Updated** (e.g., marked as lost)
- **Deleted**

The SignalR hub automatically broadcasts the updated statistics to all connected clients viewing the statistics page.

## Technical Details

- **SignalR Library**: Microsoft.AspNetCore.SignalR (built into .NET 9)
- **JavaScript Client**: CDN-hosted SignalR client library v8.0.0
- **Auto-reconnect**: Enabled with visual status indicator
- **Database**: Uses the existing `BookCopies` table with `IsLost` column

## Configuration

The feature is registered in `Program.cs`:
```csharp
// SignalR service
builder.Services.AddSignalR();

// Repository (EF mode only)
builder.Services.AddScoped<IBookCopyRepository, EfBookCopyRepository>();

// SignalR hub mapping
app.MapHub<BookCopiesStatsHub>("/bookCopiesStatsHub");
```

**Note**: This feature currently only works with Entity Framework mode. JSON repository mode is not supported for BookCopies.
