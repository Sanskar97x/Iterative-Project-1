 import { Injectable } from '@angular/core'; 
 import { HttpClient, HttpParams } from '@angular/common/http'; 
 import { Observable } from 'rxjs'; 
import { environment } from '../../../environments/environment';
 import { UserSummaryDto } from '../models/user-summary.model'; 
 @Injectable({ providedIn: 'root' }) 
 export class UserService { private readonly baseUrl = `${environment.apiUrl}/Users`; 
 constructor(private http: HttpClient) {} getByRole(role: string): Observable<UserSummaryDto[]> 
 { const params = new HttpParams().set('role', role); return this.http.get<UserSummaryDto[]>(this.baseUrl, { params }); } }