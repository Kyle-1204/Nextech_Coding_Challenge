import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HackerNewsItem, StoriesResponse } from '../models/hacker-news.model';

@Injectable({
  providedIn: 'root'
})
export class HackerNewsService {
  private readonly apiUrl = 'http://localhost:5028/api';

  constructor(private http: HttpClient) { }

  getNewestStories(page: number = 1, pageSize: number = 20, search?: string): Observable<StoriesResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<StoriesResponse>(`${this.apiUrl}/stories/newest`, { params });
  }

  getStoryById(id: number): Observable<HackerNewsItem> {
    return this.http.get<HackerNewsItem>(`${this.apiUrl}/stories/${id}`);
  }
}
