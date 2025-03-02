
# Exercise Sitefinity Project

.NETCore MVC

### Description
- User: Admin, Editor
- Module: Blog post, User
- 2 Library: Modules, Service
- 1 MVC project: DemoExcercise
- Migration: Code First
- Database: SQL SERVER
- Authentication: Identity
- Authorization: Role based
- UI: Bootstrap 4, Jquery, Toastr
- Validation: FluentValidation
- Image: Physical path
- File: Physical path
- Rich text: TIMCE

### Flow
- User: Amin
- Credential: admin@example.com / dev123@
```
Create and Delete users
Approve and Reject blog posts
Create / edit own blog posts
```
- User: Editor
- Credential: hieuden0@gmail.com / dev123@ 
```
Create / edit own blog posts
```
Blog post should have the following status:
```
Draft
Published
Rejected
```

### Module
Blog:
```
Title – textbox
Banner Image – File input, restricted to JPG, PNG and GIF and less than 5MB in size
Content – Rich text WYSWYG editor
```

### Setup

- Restore SQL SERVER: DemoExcercise\dabase\sf15.bak with "sf15" name
- Update ConnectionString config in appsettings.Development.json.config with server connnection: <add connectionString="data source=<server>;UID=<username>;PWD=<password>;initial catalog=dev123@;MultipleActiveResultSets=true;TrustServerCertificate=True" />
- Open project by visual studio then run it (or we can setup it on IIS with local domain)




