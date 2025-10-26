import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaginationComponent } from './pagination.component';

describe('PaginationComponent', () => {
  let component: PaginationComponent;
  let fixture: ComponentFixture<PaginationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaginationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaginationComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit page change', () => {
    spyOn(component.pageChange, 'emit');
    component.currentPage = 1;
    component.totalPages = 5;

    component.onPageChange(2);

    expect(component.pageChange.emit).toHaveBeenCalledWith(2);
  });

  it('should not emit page change for invalid pages', () => {
    spyOn(component.pageChange, 'emit');
    component.currentPage = 1;
    component.totalPages = 5;

    component.onPageChange(0);
    component.onPageChange(6);
    component.onPageChange(1); // current page

    expect(component.pageChange.emit).not.toHaveBeenCalled();
  });

  it('should calculate visible pages correctly', () => {
    component.currentPage = 3;
    component.totalPages = 10;

    const visiblePages = component.getVisiblePages();

    expect(visiblePages).toEqual([1, 2, 3, 4, 5]);
  });

  it('should calculate start and end items correctly', () => {
    component.currentPage = 2;
    component.pageSize = 20;
    component.totalCount = 100;

    expect(component.startItem).toBe(21);
    expect(component.endItem).toBe(40);
  });

  it('should handle last page end item correctly', () => {
    component.currentPage = 5;
    component.pageSize = 20;
    component.totalCount = 85; // Not divisible by page size

    expect(component.endItem).toBe(85);
  });
});
