import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  public baseUrl = environment.baseUrl;
  private http = inject(HttpClient)

  getNewestStories() {
    return this.http.get(`${this.baseUrl}/HackerNews/newest/`);
  }
  
}
