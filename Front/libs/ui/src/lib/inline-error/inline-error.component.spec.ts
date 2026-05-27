import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InlineErrorComponent } from './inline-error.component';

describe('InlineErrorComponent', () => {
  let component: InlineErrorComponent;
  let fixture: ComponentFixture<InlineErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InlineErrorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(InlineErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
