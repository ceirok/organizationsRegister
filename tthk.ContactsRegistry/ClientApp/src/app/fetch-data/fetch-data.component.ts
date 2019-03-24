import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public contacts: IContact[];
  public searchTerm: string = "";

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {
    this._reloadList();
  }

  private _reloadList() {
    this._http.get<IContact[]>(this._baseUrl + 'api/Contacts', {
      params: {
        term: this.searchTerm
      }
    })
      .subscribe(result => {
        this.contacts = result;
      }, error => console.error(error));
  }

  public delete(contact: IContact, i: number) {
    this._http.delete(this._baseUrl + `api/Contacts/${contact.id}`)
      .subscribe(result => {
        this.contacts.splice(i, 1);
      }, error => console.error(error));
  }

  public search() {
    this._reloadList();
  }
}

interface IContact {
  id: string,
  name: string;
  defaultPhoneNumber: string;
  defaultEmail: string;
}
