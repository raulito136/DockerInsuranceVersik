import { ComponentFixture, TestBed, tick, fakeAsync } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { CoverageTypeListComponent } from './coverage-type-list.component';
import { CoverageTypeService } from 'mfe-reference-data/src/app/services/coverage-type.service';
import { of, throwError } from 'rxjs';
import { CoverageType } from 'mfe-reference-data/src/app/models/coverage-type.model';
import { findMatchingDirectivesAndPipes } from '@angular/compiler';

describe('CoverageTypeListComponent', () => {
  let component: CoverageTypeListComponent;
  let fixture: ComponentFixture<CoverageTypeListComponent>;
  let coverageTypeServiceSpy: jest.Mocked<CoverageTypeService>;
  beforeEach(async () => {
    const spy={
      getCoverageTypes: jest.fn(),
      deleteCoverageType: jest.fn(),
      activateCoverageType: jest.fn()
    }
    await TestBed.configureTestingModule({
      imports: [CoverageTypeListComponent],
      providers: [
        {provide: CoverageTypeService, useValue: spy},
        {provide: ActivatedRoute, useValue: {}}
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CoverageTypeListComponent);
    component = fixture.componentInstance;
    coverageTypeServiceSpy = TestBed.inject(CoverageTypeService) as jest.Mocked<CoverageTypeService>;
  });

  it('should create', () => {
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data: []} as any));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load coverage types on init', ()=>{
    const mockClaims: CoverageType[] = [
      {id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: true},
      {id: 2, code: 'C2', name: 'Coverage 2', description: '', isActive: false}
    ];
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data: mockClaims} as any));
    fixture.detectChanges();

    expect(coverageTypeServiceSpy.getCoverageTypes).toHaveBeenCalledWith(false);
    expect(component.coverageTypes()).toEqual(mockClaims);
    expect(component.loading()).toBe(false);
  });

  it('should handle error when loading coverage types', ()=>{
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(throwError(()=>'Error loading coverage types'));

    fixture.detectChanges();

    expect(component.errores()).toEqual(['Error loading coverage types']);
    expect(component.loading()).toBe(false);
  });

  it('should reload data when showAll changes', ()=>{
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data: []} as any));
    fixture.detectChanges();

    coverageTypeServiceSpy.getCoverageTypes.mockClear();

    component.onShowAllChange(true);

    expect(component.showAll()).toBe(true);
    expect(coverageTypeServiceSpy.getCoverageTypes).toHaveBeenCalledWith(true);
  });


  it('should ask for confirmation and delete when toggling an active status', fakeAsync(()=>{
    const mockCoverage:CoverageType={id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: true};
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data: [mockCoverage]} as any));
    coverageTypeServiceSpy.deleteCoverageType.mockReturnValue(of({} as any));

    fixture.detectChanges();
    coverageTypeServiceSpy.getCoverageTypes.mockClear();

    component.toggle(mockCoverage);
    expect(component.isConfirming).toBe(true);

    component.handleConfirmation(true);
    tick();

    expect(coverageTypeServiceSpy.deleteCoverageType).toHaveBeenCalledWith(1);
    expect(coverageTypeServiceSpy.getCoverageTypes).toHaveBeenCalledWith(false);
  }));

  it('should not delete if confirmation is cancelled', fakeAsync(()=>{
    const mockCoverage:CoverageType={id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: true};
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data: [mockCoverage]}as any));

    fixture.detectChanges();

    component.toggle(mockCoverage);
    component.handleConfirmation(false);
    tick();

    expect(coverageTypeServiceSpy.deleteCoverageType).not.toHaveBeenCalled;
  }));

  it('should activate when toggling a inactive status directly without confirmation', ()=>{
    const mockCoverage:CoverageType={id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: false};
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data:[mockCoverage]}) as any);
    coverageTypeServiceSpy.activateCoverageType.mockReturnValue(of({} as any));

    fixture.detectChanges();

    component.toggle(mockCoverage)

    expect(coverageTypeServiceSpy.activateCoverageType).toHaveBeenCalledWith(1);
    expect(coverageTypeServiceSpy.getCoverageTypes).toHaveBeenCalledWith(false);
  });

  it('should handle error when activating a status fails', ()=>{
    const mockCoverage:CoverageType={id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: false};
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data:[mockCoverage]} as any));
    coverageTypeServiceSpy.activateCoverageType.mockReturnValue(throwError(()=>'Activate error'));

    fixture.detectChanges();

    component.toggle(mockCoverage);

    expect(component.errores()).toEqual(['Activate error']);


  });

  it('should handle error when deleting a status fails', fakeAsync(()=>{
    const mockCoverage:CoverageType={id: 1, code: 'C1', name: 'Coverage 1', description: '', isActive: true};
    coverageTypeServiceSpy.getCoverageTypes.mockReturnValue(of({data:[mockCoverage]}) as any);
    coverageTypeServiceSpy.deleteCoverageType.mockReturnValue(throwError(()=>'Delete error'));

    fixture.detectChanges();

    component.toggle(mockCoverage);
    component.handleConfirmation(true);
    tick();

    expect(component.errores()).toEqual(['Delete error']);
  }));

});
