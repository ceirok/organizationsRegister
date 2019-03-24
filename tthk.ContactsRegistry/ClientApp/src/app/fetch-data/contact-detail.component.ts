import { ActivatedRoute, Router } from '@angular/router';
import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isDate } from 'util';

@Component({
  selector: 'contact-detail',
  templateUrl: './contact-detail.component.html'
})
export class ContactDetailComponent {

  public loading = true;

  public contacts: IContact[];

  public model = {
    phoneNumbers: [],
    emails: []
  } as IContact;

  private _defaultEmail: IEmail;
  private _defaultPhone: IPhoneNumber;

  public phoneTypes = ["Work", "Home"];

  public emailTypes = ["Work", "Spam", "Fun"];

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string, private _route: ActivatedRoute, private _router: Router) {

  }

  ngOnInit() {
    this.model.id = this._route.snapshot.params['id'];

    console.log(this.model);

    if (this.model.id) {
      this._http.get<IContact>(this._baseUrl + `api/Contacts/${this.model.id}`)
        .subscribe(result => {
          Object.assign(this.model, result);

          for (var i = 0; i < this.model.phoneNumbers.length; i++) {
            if (this.model.phoneNumbers[i].isDefault) {
              this._defaultPhone = this.model.phoneNumbers[i];
              break;
            }
          }

          for (var i = 0; i < this.model.emails.length; i++) {
            if (this.model.emails[i].isDefault) {
              this._defaultEmail = this.model.emails[i];
              break;
            }
          }

          this.loading = false;
        }, error => console.error(error));
    }
    else {
      this.loading = false;
    }
  }

  public addNumber() {
    this.model.phoneNumbers.push({} as IPhoneNumber);
  }

  public removeNumber(i) {
    this.model.phoneNumbers.splice(i, 1);
  }

  public addEmail() {
    this.model.emails.push({} as IEmail);
  }

  public removeEmail(i) {
    this.model.emails.splice(i, 1);
  }

  public setDefaultEmail(email: IEmail) {
    if (this._setDefault(email, this._defaultEmail)) {
      this._defaultEmail = email;
    }
  }

  public setDefaultPhone(phone: IPhoneNumber) {
    if (this._setDefault(phone, this._defaultPhone)) {
      this._defaultPhone = phone;
    }
  }

  private _setDefault(item: IHasDefault, current: IHasDefault) {
    if (!item.isDefault) {
      return false;
    }

    if (current) {
      current.isDefault = false;
    }
    return true;
  }
  
  public save() {

    if (this.model.id) {

      this._http.post<any>(this._baseUrl + `api/Contacts/${this.model.id}`, this.model)
        .subscribe(result => {
          this._router.navigate(["fetch-data"]);
        }, error => console.error(error));

    }
    else {

      this._http.post<any>(this._baseUrl + `api/Contacts/add`, this.model)
        .subscribe(result => {
          this._router.navigate(["fetch-data"]);
        }, error => console.error(error));
    }
  }

}

interface IContact {
  id: string;
  name: string;
  initials: string;
  phoneNumber: string;
  email: string;
  phoneNumbers: IPhoneNumber[],
  emails: IEmail[]
}

interface IEmail extends IHasDefault {
  email: string;
  type: string;
}

interface IPhoneNumber extends IHasDefault {
  number: string;
  type: string;
}

interface IContact {
  id: string,
  name: string;
  defaultPhoneNumber: number;
  defaultEmail: number;
}

interface IHasDefault {
  isDefault: boolean;
}
