import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RegionFormComponent } from './region-form.component';
import { RegionService } from 'mfe-reference-data/src/app/services/region.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('RegionFormComponent', () => {
  let component: RegionFormComponent;
  let fixture: ComponentFixture<RegionFormComponent>;
  let regionServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    regionServiceMock = {
      getRegionsById: jest.fn().mockReturnValue(of({ data: { id: 1, code: 'R1', name: 'Region 1', isActive: true } })),
      createRegion: jest.fn().mockReturnValue(of({})),
      updateRegion: jest.fn().mockReturnValue(of({})),
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
      imports: [RegionFormComponent, ReactiveFormsModule, BrowserAnimationsModule],
      providers: [
        { provide: RegionService, useValue: regionServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should initialize in create mode', () => {
      expect(component.isEditMode).toBeFalsy();
    });

    it('should initialize in edit mode and fetch data', () => {
      const activatedRoute = TestBed.inject(ActivatedRoute);
      activatedRoute.snapshot.paramMap.get = jest.fn().mockReturnValue('1');
      
      component.ngOnInit();
      
      expect(component.isEditMode).toBeTruthy();
      expect(regionServiceMock.getRegionsById).toHaveBeenCalledWith(1);
      expect(component.myForm.value).toEqual({
        id: 1,
        code: 'R1',
        name: 'Region 1',
        isActive: true
      });
    });
  });

  describe('onSubmitForm', () => {
    it('should not submit if form is invalid', () => {
      component.myForm.get('code')?.setValue('');
      component.onSubmitForm(new Event('submit'));
      expect(regionServiceMock.createRegion).not.toHaveBeenCalled();
      expect(regionServiceMock.updateRegion).not.toHaveBeenCalled();
    });

    it('should create region on submit in create mode', () => {
      component.isEditMode = false;
      component.myForm.patchValue({
        code: 'NEW',
        name: 'New Name',
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(regionServiceMock.createRegion).toHaveBeenCalledWith({
        code: 'NEW',
        name: 'New Name',
      });
      expect(routerMock.navigate).toHaveBeenCalledWith(['../'], { relativeTo: expect.anything() });
    });

    it('should update region on submit in edit mode', () => {
      component.isEditMode = true;
      component.myForm.patchValue({
        id: 1,
        code: 'UPD',
        name: 'Upd Name',
        isActive: false
      });
      
      component.onSubmitForm(new Event('submit'));
      
      expect(regionServiceMock.updateRegion).toHaveBeenCalledWith(1, {
        code: 'UPD',
        name: 'Upd Name',
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
