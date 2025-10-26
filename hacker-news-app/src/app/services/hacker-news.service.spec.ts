import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HackerNewsService } from './hacker-news.service';
import { HackerNewsItem, StoriesResponse } from '../models/hacker-news.model';

describe('HackerNewsService', () => {
  let service: HackerNewsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [HackerNewsService]
    });
    service = TestBed.inject(HackerNewsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch newest stories', () => {
    const mockResponse: StoriesResponse = {
      stories: [
        {
          id: 1,
          title: 'Test Story',
          url: 'https://example.com',
          by: 'testuser',
          time: 1609459200,
          score: 100,
          descendants: 5,
          type: 'story',
          createdAt: new Date()
        }
      ],
      totalCount: 1,
      page: 1,
      pageSize: 20,
      totalPages: 1
    };

    service.getNewestStories().subscribe(response => {
      expect(response).toEqual(mockResponse);
      expect(response.stories.length).toBe(1);
      expect(response.stories[0].title).toBe('Test Story');
    });

    const req = httpMock.expectOne('http://localhost:5028/api/stories/newest?page=1&pageSize=20');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch newest stories with search parameters', () => {
    const searchTerm = 'Angular';
    const mockResponse: StoriesResponse = {
      stories: [],
      totalCount: 0,
      page: 1,
      pageSize: 20,
      totalPages: 0
    };

    service.getNewestStories(1, 20, searchTerm).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`http://localhost:5028/api/stories/newest?page=1&pageSize=20&search=${searchTerm}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch story by id', () => {
    const mockStory: HackerNewsItem = {
      id: 123,
      title: 'Test Story',
      url: 'https://example.com',
      by: 'testuser',
      time: 1609459200,
      score: 100,
      descendants: 5,
      type: 'story',
      createdAt: new Date()
    };

    service.getStoryById(123).subscribe(story => {
      expect(story).toEqual(mockStory);
      expect(story.id).toBe(123);
    });

    const req = httpMock.expectOne('http://localhost:5028/api/stories/123');
    expect(req.request.method).toBe('GET');
    req.flush(mockStory);
  });
});
