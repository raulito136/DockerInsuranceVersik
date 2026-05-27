import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PolicyHolderFormComponent } from './policy-holder-form.component';

describe('PolicyHolderFormComponent', () => {
  let component: PolicyHolderFormComponent;
  let fixture: ComponentFixture<PolicyHolderFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PolicyHolderFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PolicyHolderFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
