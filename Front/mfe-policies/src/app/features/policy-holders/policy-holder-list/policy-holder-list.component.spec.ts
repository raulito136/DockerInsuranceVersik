import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PolicyHolderListComponent } from './policy-holder-list.component';

describe('PolicyHolderListComponent', () => {
  let component: PolicyHolderListComponent;
  let fixture: ComponentFixture<PolicyHolderListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PolicyHolderListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PolicyHolderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
