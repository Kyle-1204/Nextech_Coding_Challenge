export interface HackerNewsItem {
  id: number;
  title?: string;
  url?: string;
  by?: string;
  time: number;
  score: number;
  descendants: number;
  type?: string;
  createdAt: Date;
}

export interface StoriesResponse {
  stories: HackerNewsItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}