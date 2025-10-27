import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})

//Default vals
export class PaginationComponent {
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() totalCount: number = 0;
  @Input() pageSize: number = 20;
  
  @Output() pageChange = new EventEmitter<number>();

  // Structure inspired by:
  // https://stackoverflow.com/questions/68556582/typescript-pagination

  // Prevents navigation to invalid pages - current and out of bounds
  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.pageChange.emit(page);
    }
  }

  // Creates finite window of visible pages based on current page
  getVisiblePages(): number[] {
    const pages: number[] = [];
    const startPage = Math.max(1, this.currentPage - 2);
    const endPage = Math.min(this.totalPages, this.currentPage + 2);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  // Index of fist item - either start of page or 1
  get startItem(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  // Index of last item - either end of page or total count
  get endItem(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }
}
