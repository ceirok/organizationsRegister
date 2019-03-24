# Kontaktide register

## DB

Contact
- Id
- Name
- PhoneNumbers [ { number: string, type: enum, isDefault: boolean} ]
- Emails [ { email: string, type: enum, isDefault: boolean }]

Contact (Easy)
- Id
- Name
- PhoneNumber
- Email 

## Migration

- add field "Initials"

## API


### List (with filtering by term - search from name, phone nr, email)

[GET] /api/contacts?term={}
[{
	id,
	name,
	defaultPhoneNumber,
	defaultEmail
}]

### Get item

[GET] /api/contacts/{id}

### Create

[POST ]/api/contacts/add

### Update

[POST] /api/contacts/{id}

### Delete

[DELETE] /api/contacts/{id}

