import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PolicyTypeFormComponent } from './policy-type-form.component';
import { PolicyTypeService } from 'mfe-reference-data/src/app/services/policy-type.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('PolicyTypeFormComponent', () => {
  let component: PolicyTypeFormComponent;
  let fixture: ComponentFixture<PolicyTypeFormComponent>;
  let policyServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    policyServiceMock = {
      getPolicyTypeById: jest.fn().mockReturnValue(of({ data: { id: 1, code: 'P1', name: 'Policy 1', description: 'Desc', isActive: true } })),
      createPolicyType: jest.fn().mockReturnValue(of({})),
      updatePolicyType: jest.fn().mockReturnValue(of({})),
    };

    routerMock = {
      navigate: jest.fn(),
    };

    const activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jest.fn().mockReturnValue(null),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [PolicyTypeFormComponent, ReactiveFormsModule, BrowserAnimationsModule],
      providers: [
        { provide: PolicyTypeService, useValue: policyServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PolicyTypeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should initialize in create mode', () => {
      expect(component.isEditMode).toBeFalsy();
      expect(component.myForm.get('code')?.hasValidator).toBeTruthy();
    });

    it('should initialize in edit mode and fetch data', () => {
      const activatedRoute = TestBed.inject(ActivatedRoute);
      activatedRoute.snapshot.paramMap.get = jest.fn().mockReturnValue('1');
      
      component.ngOnInit();
      
      expect(component.isEditMode).toBeTruthy();
      expect(policyServiceMock.getPolicyTypeById).toHaveBeenCalledWith(1);
      expect(component.myForm.value).toEqual({
        id: 1,
        code: 'P1',
        name: 'Policy 1',
        description: 'Desc',
        isActive: true
      });
    });
  });

  describe('onSubmitForm', () => {
    it('should not submit if form is invalid', () => {
      component.myForm.get('code')?.setValue('');
      component.onSubmitForm(new Event('submit'));
      expect(policyServiceMock.createPolicyType).not.toHaveBeenCalled();
      expect(policyServiceMock.updatePolicyType).not.toHaveBeenCalled();
    });

    it('should create policy type on submit in create mode', () => {
      component.isEditMode = false;
      component.myForm.patchValue({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc',
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(policyServiceMock.createPolicyType).toHaveBeenCalledWith({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc'
      });
      expect(routerMock.navigate).toHaveBeenCalledWith(['../'], { relativeTo: expect.anything() });
    });

    it('should update policy type on submit in edit mode', () => {
      component.isEditMode = true;
      component.myForm.patchValue({
        id: 1,
        code: 'UPD',
        name: 'Upd Name',
        description: 'Upd Desc',
        isActive: false
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(policyServiceMock.updatePolicyType).toHaveBeenCalledWith(1, {
        name: 'Upd Name',
        description: 'Upd Desc',
        isActive: false
      });
      expect(routerMock.navigate).toHaveBeenCalledWith(['../../'], { relativeTo: expect.anything() });
    });
  });

  describe('onCancel', () => {
    it('should navigate to parent route in create mode', () => {
      component.isEditMode = false;
      component.onCancel();
      expect(routerMock.navigate).toHaveBeenCalledWith(['../'], { relativeTo: expect.anything() });
    });

    it('should navigate to grandparent route in edit mode', () => {
      component.isEditMode = true;
      component.onCancel();
      expect(routerMock.navigate).toHaveBeenCalledWith(['../../'], { relativeTo: expect.anything() });
    });
  });
});
