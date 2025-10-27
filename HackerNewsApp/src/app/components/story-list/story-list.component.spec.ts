import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { StoryListComponent } from './story-list.component';
import { HackerNewsService } from '../../services/hacker-news.service';
import { StoriesResponse } from '../../models/hacker-news.model';

describe('StoryListComponent', () => {
  let component: StoryListComponent;
  let fixture: ComponentFixture<StoryListComponent>;
  let hackerNewsService: jasmine.SpyObj<HackerNewsService>;

  const mockStoriesResponse: StoriesResponse = {
    stories: [
      {
        id: 1,
        title: 'Test Story 1',
        url: 'https://example.com/1',
        by: 'user1',
        time: 1609459200,
        score: 100,
        descendants: 5,
        type: 'story',
        createdAt: new Date(1609459200 * 1000)
      },
      {
        id: 2,
        title: 'Test Story 2',
        url: 'https://example.com/2',
        by: 'user2',
        time: 1609459300,
        score: 50,
        descendants: 2,
        type: 'story',
        createdAt: new Date(1609459300 * 1000)
      }
    ],
    totalCount: 2,
    page: 1,
    pageSize: 20,
    totalPages: 1
  };

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('HackerNewsService', ['getNewestStories']);

    await TestBed.configureTestingModule({
      imports: [StoryListComponent, HttpClientTestingModule],
      providers: [
        { provide: HackerNewsService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StoryListComponent);
    component = fixture.componentInstance;
    hackerNewsService = TestBed.inject(HackerNewsService) as jasmine.SpyObj<HackerNewsService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stories on init', () => {
    hackerNewsService.getNewestStories.and.returnValue(of(mockStoriesResponse));

    component.ngOnInit();

    expect(hackerNewsService.getNewestStories).toHaveBeenCalledWith(1, 20, '');
    expect(component.stories.length).toBe(2);
    expect(component.loading).toBe(false);
    expect(component.error).toBe(null);
  });

  it('should handle search', () => {
    hackerNewsService.getNewestStories.and.returnValue(of({
      ...mockStoriesResponse,
      stories: [mockStoriesResponse.stories[0]]
    }));

    component.onSearchChange('Test');

    expect(hackerNewsService.getNewestStories).toHaveBeenCalledWith(1, 20, 'Test');
    expect(component.currentPage).toBe(1);
    expect(component.searchTerm).toBe('Test');
  });

  it('should handle page change', () => {
    hackerNewsService.getNewestStories.and.returnValue(of(mockStoriesResponse));

    component.onPageChange(2);

    expect(hackerNewsService.getNewestStories).toHaveBeenCalledWith(2, 20, '');
    expect(component.currentPage).toBe(2);
  });

  it('should handle errors', () => {
    hackerNewsService.getNewestStories.and.returnValue(throwError(() => new Error('Network error')));

    component.loadStories();

    expect(component.loading).toBe(false);
    expect(component.error).toBe('Failed to load stories. Please try again.');
  });

  it('should detect valid URLs', () => {
    const storyWithUrl = { ...mockStoriesResponse.stories[0] };
    const storyWithoutUrl = { ...mockStoriesResponse.stories[0], url: undefined };

    expect(component.hasValidUrl(storyWithUrl)).toBe(true);
    expect(component.hasValidUrl(storyWithoutUrl)).toBe(false);
  });

  it('should format time ago correctly', () => {
    const now = new Date();
    const oneHourAgo = new Date(now.getTime() - 60 * 60 * 1000);
    const oneDayAgo = new Date(now.getTime() - 24 * 60 * 60 * 1000);

    expect(component.getTimeAgo(oneHourAgo)).toContain('hour');
    expect(component.getTimeAgo(oneDayAgo)).toContain('day');
  });
});
