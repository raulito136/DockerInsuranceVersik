import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PolicyHolderDetailComponent } from './policy-holder-detail.component';

describe('PolicyHolderDetailComponent', () => {
  let component: PolicyHolderDetailComponent;
  let fixture: ComponentFixture<PolicyHolderDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PolicyHolderDetailComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PolicyHolderDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
