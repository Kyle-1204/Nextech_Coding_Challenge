import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HackerNewsService } from '../../services/hacker-news.service';
import { HackerNewsItem, StoriesResponse } from '../../models/hacker-news.model';
import { SearchBarComponent } from '../search-bar/search-bar.component';
import { PaginationComponent } from '../pagination/pagination.component';

@Component({
  selector: 'app-story-list',
  standalone: true,
  imports: [CommonModule, SearchBarComponent, PaginationComponent],
  templateUrl: './story-list.component.html',
  styleUrls: ['./story-list.component.css']
})
export class StoryListComponent implements OnInit {
  stories: HackerNewsItem[] = [];
  currentPage: number = 1;
  totalPages: number = 1;
  totalCount: number = 0;
  pageSize: number = 20;
  searchTerm: string = '';
  loading: boolean = false;
  error: string | null = null;

  constructor(private hackerNewsService: HackerNewsService) {}

  ngOnInit(): void {
    this.loadStories();
  }

  loadStories(): void {
    this.loading = true;
    this.error = null;

    // Loads stories from Hacker News Service
    this.hackerNewsService.getNewestStories(this.currentPage, this.pageSize, this.searchTerm)
      .subscribe({
        next: (response: StoriesResponse) => {
          this.stories = response.stories.map(story => ({
            ...story,
            createdAt: new Date(story.time * 1000) // Convert Unix timestamp to Date
          }));
          this.totalPages = response.totalPages;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading stories:', err);
          this.error = 'Failed to load stories. Please try again.';
          this.loading = false;
        }
      });
  }

  onSearchChange(searchTerm: string): void {
    this.searchTerm = searchTerm;
    this.currentPage = 1; // Reset to first page when searching
    this.loadStories();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadStories();
    // Scroll to top of the page
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  // "Watch out for stories that do not have hyperlinks!"
  hasValidUrl(story: HackerNewsItem): boolean {
    return !!(story.url && story.url.trim());
  }

  // Source:
  // https://stackoverflow.com/questions/3177836/how-to-format-time-since-xxx-e-g-4-minutes-ago-similar-to-stack-exchange-site
  getTimeAgo(date: Date): string {
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diffInSeconds < 60) {
      return 'just now';
    } else if (diffInSeconds < 3600) {
      const minutes = Math.floor(diffInSeconds / 60);
      return `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
    } else if (diffInSeconds < 86400) {
      const hours = Math.floor(diffInSeconds / 3600);
      return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
    } else {
      const days = Math.floor(diffInSeconds / 86400);
      return `${days} day${days !== 1 ? 's' : ''} ago`;
    }
  }

  trackByStoryId(index: number, story: HackerNewsItem): number {
    return story.id;
  }
}
