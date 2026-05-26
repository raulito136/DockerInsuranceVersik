import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClaimStatusFormComponent } from './claim-status-form.component';
import { ClaimStatusService } from 'mfe-reference-data/src/app/services/claim-status.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('ClaimStatusFormComponent', () => {
  let component: ClaimStatusFormComponent;
  let fixture: ComponentFixture<ClaimStatusFormComponent>;
  let claimServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    claimServiceMock = {
      getClaimStatusById: jest.fn().mockReturnValue(of({ data: { id: 1, code: 'C1', name: 'Claim 1', description: 'Desc', isActive: true } })),
      createClaimStatus: jest.fn().mockReturnValue(of({})),
      updateClaimStatus: jest.fn().mockReturnValue(of({})),
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
      imports: [ClaimStatusFormComponent, ReactiveFormsModule, BrowserAnimationsModule],
      providers: [
        { provide: ClaimStatusService, useValue: claimServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ClaimStatusFormComponent);
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
      expect(claimServiceMock.getClaimStatusById).toHaveBeenCalledWith(1);
      expect(component.myForm.value).toEqual({
        id: 1,
        code: 'C1',
        name: 'Claim 1',
        description: 'Desc',
        isActive: true
      });
    });
  });

  describe('onSubmitForm', () => {
    it('should not submit if form is invalid', () => {
      component.myForm.get('code')?.setValue('');
      component.onSubmitForm(new Event('submit'));
      expect(claimServiceMock.createClaimStatus).not.toHaveBeenCalled();
      expect(claimServiceMock.updateClaimStatus).not.toHaveBeenCalled();
    });

    it('should create claim status on submit in create mode', () => {
      component.isEditMode = false;
      component.myForm.patchValue({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc',
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(claimServiceMock.createClaimStatus).toHaveBeenCalledWith({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc'
      });
      expect(routerMock.navigate).toHaveBeenCalledWith(['../'], { relativeTo: expect.anything() });
    });

    it('should update claim status on submit in edit mode', () => {
      component.isEditMode = true;
      component.myForm.patchValue({
        id: 1,
        code: 'UPD',
        name: 'Upd Name',
        description: 'Upd Desc',
        isActive: false
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(claimServiceMock.updateClaimStatus).toHaveBeenCalledWith(1, {
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
