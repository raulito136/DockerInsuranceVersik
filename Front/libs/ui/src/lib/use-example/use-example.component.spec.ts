import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UseExampleComponent } from './use-example.component';

describe('UseExampleComponent', () => {
  let component: UseExampleComponent;
  let fixture: ComponentFixture<UseExampleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UseExampleComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(UseExampleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
