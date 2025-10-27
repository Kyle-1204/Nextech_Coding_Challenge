import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { SearchBarComponent } from './search-bar.component';

describe('SearchBarComponent', () => {
  let component: SearchBarComponent;
  let fixture: ComponentFixture<SearchBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchBarComponent, FormsModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit search change on search', () => {
    spyOn(component.searchChange, 'emit');
    component.searchTerm = 'test search';

    component.onSearch();

    expect(component.searchChange.emit).toHaveBeenCalledWith('test search');
  });

  it('should emit empty string on clear', () => {
    spyOn(component.searchChange, 'emit');
    component.searchTerm = 'test search';

    component.onClear();

    expect(component.searchTerm).toBe('');
    expect(component.searchChange.emit).toHaveBeenCalledWith('');
  });

  it('should trigger search on enter key', () => {
    spyOn(component, 'onSearch');
    const input = fixture.debugElement.nativeElement.querySelector('input');
    
    const event = new KeyboardEvent('keyup', { key: 'Enter' });
    input.dispatchEvent(event);

    expect(component.onSearch).toHaveBeenCalled();
  });
});
