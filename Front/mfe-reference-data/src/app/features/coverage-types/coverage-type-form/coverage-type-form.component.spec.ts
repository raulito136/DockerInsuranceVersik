import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CoverageTypeFormComponent } from './coverage-type-form.component';
import { CoverageTypeService } from 'mfe-reference-data/src/app/services/coverage-type.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('CoverageTypeFormComponent', () => {
  let component: CoverageTypeFormComponent;
  let fixture: ComponentFixture<CoverageTypeFormComponent>;
  let coverageServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    coverageServiceMock = {
      getCoverageTypeById: jest.fn().mockReturnValue(of({ data: { id: 1, code: 'C1', name: 'Coverage 1', description: 'Desc', isActive: true } })),
      createCoverageType: jest.fn().mockReturnValue(of({})),
      updateCoverageType: jest.fn().mockReturnValue(of({})),
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
      imports: [CoverageTypeFormComponent, ReactiveFormsModule, BrowserAnimationsModule],
      providers: [
        { provide: CoverageTypeService, useValue: coverageServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CoverageTypeFormComponent);
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
      expect(coverageServiceMock.getCoverageTypeById).toHaveBeenCalledWith(1);
      expect(component.myForm.value).toEqual({
        id: 1,
        code: 'C1',
        name: 'Coverage 1',
        description: 'Desc',
        isActive: true
      });
    });
  });

  describe('onSubmitForm', () => {
    it('should not submit if form is invalid', () => {
      component.myForm.get('code')?.setValue('');
      component.onSubmitForm(new Event('submit'));
      expect(coverageServiceMock.createCoverageType).not.toHaveBeenCalled();
      expect(coverageServiceMock.updateCoverageType).not.toHaveBeenCalled();
    });

    it('should create coverage type on submit in create mode', () => {
      component.isEditMode = false;
      component.myForm.patchValue({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc',
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(coverageServiceMock.createCoverageType).toHaveBeenCalledWith({
        code: 'NEW',
        name: 'New Name',
        description: 'New Desc'
      });
      expect(routerMock.navigate).toHaveBeenCalledWith(['../'], { relativeTo: expect.anything() });
    });

    it('should update coverage type on submit in edit mode', () => {
      component.isEditMode = true;
      component.myForm.patchValue({
        id: 1,
        code: 'UPD',
        name: 'Upd Name',
        description: 'Upd Desc',
        isActive: false
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(coverageServiceMock.updateCoverageType).toHaveBeenCalledWith(1, {
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
