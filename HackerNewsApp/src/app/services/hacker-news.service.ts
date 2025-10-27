import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HackerNewsItem, StoriesResponse } from '../models/hacker-news.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HackerNewsService {
  constructor(private http: HttpClient) { }

  getNewestStories(page: number = 1, pageSize: number = 20, search?: string): Observable<StoriesResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<StoriesResponse>(`${environment.apiBaseUrl}/stories/newest`, { params });
  }

  getStoryById(id: number): Observable<HackerNewsItem> {
    return this.http.get<HackerNewsItem>(`${environment.apiBaseUrl}/stories/${id}`);
  }
}
