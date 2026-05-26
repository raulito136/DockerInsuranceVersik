import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RegionListComponent } from './region-list.component';
import { RegionService } from 'mfe-reference-data/src/app/services/region.service';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Region } from 'mfe-reference-data/src/app/models/region.model';

describe('RegionListComponent', () => {
  let component: RegionListComponent;
  let fixture: ComponentFixture<RegionListComponent>;
  let regionServiceSpy: jest.Mocked<RegionService>;

  beforeEach(async () => {
    const spy = {
      getRegions: jest.fn(),
      deleteRegion: jest.fn(),
      activateRegion: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [RegionListComponent],
      providers: [
        { provide: RegionService, useValue: spy },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RegionListComponent);
    component = fixture.componentInstance;
    regionServiceSpy = TestBed.inject(RegionService) as unknown as jest.Mocked<RegionService>;
  });

  it('should create', () => {
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load regions on init', () => {
    const mockRegions: Region[] = [
      { id: 1, code: 'R1', name: 'Region 1', isActive: true },
      { id: 2, code: 'R2', name: 'Region 2', isActive: false }
    ];
    regionServiceSpy.getRegions.mockReturnValue(of({ data: mockRegions } as any));
    
    fixture.detectChanges();
    
    expect(regionServiceSpy.getRegions).toHaveBeenCalledWith(false);
    expect(component.regions()).toEqual(mockRegions);
    expect(component.loading()).toBe(false);
  });

  it('should handle error when loading regions', () => {
    regionServiceSpy.getRegions.mockReturnValue(throwError(() => 'Error loading'));
    
    fixture.detectChanges();
    
    expect(component.errores()).toEqual(['Error loading']);
    expect(component.loading()).toBe(false);
  });

  it('should reload data when showAll changes', () => {
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges();
    
    regionServiceSpy.getRegions.mockClear();
    
    component.onShowAllChange(true);
    
    expect(component.showAll()).toBe(true);
    expect(regionServiceSpy.getRegions).toHaveBeenCalledWith(true);
  });

  it('should ask for confirmation and delete when toggling an active region', fakeAsync(() => {
    const mockRegion: Region = { id: 1, code: 'R1', name: 'Region 1', isActive: true };
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [mockRegion] } as any));
    regionServiceSpy.deleteRegion.mockReturnValue(of({} as any));
    
    fixture.detectChanges();
    regionServiceSpy.getRegions.mockClear();
    
    component.toggle(mockRegion);
    expect(component.isConfirming).toBe(true);
    
    component.handleConfirmation(true);
    tick(); 
    
    expect(regionServiceSpy.deleteRegion).toHaveBeenCalledWith(1);
    expect(regionServiceSpy.getRegions).toHaveBeenCalledWith(false); 
  }));

  it('should not delete if confirmation is cancelled', fakeAsync(() => {
    const mockRegion: Region = { id: 1, code: 'R1', name: 'Region 1', isActive: true };
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [mockRegion] } as any));
    
    fixture.detectChanges();
    
    component.toggle(mockRegion);
    component.handleConfirmation(false);
    tick();
    
    expect(regionServiceSpy.deleteRegion).not.toHaveBeenCalled();
  }));

  it('should activate when toggling an inactive region directly without confirmation', () => {
    const mockRegion: Region = { id: 1, code: 'R1', name: 'Region 1', isActive: false };
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [mockRegion] } as any));
    regionServiceSpy.activateRegion.mockReturnValue(of({} as any));
    
    fixture.detectChanges();
    regionServiceSpy.getRegions.mockClear();
    
    component.toggle(mockRegion);
    
    expect(regionServiceSpy.activateRegion).toHaveBeenCalledWith(1);
    expect(regionServiceSpy.getRegions).toHaveBeenCalledWith(false);
  });
  
  it('should handle error when activating a region fails', () => {
    const mockRegion: Region = { id: 1, code: 'R1', name: 'Region 1', isActive: false };
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [mockRegion] } as any));
    regionServiceSpy.activateRegion.mockReturnValue(throwError(() => 'Activate error'));
    
    fixture.detectChanges();
    
    component.toggle(mockRegion);
    
    expect(component.errores()).toEqual(['Activate error']);
  });

  it('should handle error when deleting a region fails', fakeAsync(() => {
    const mockRegion: Region = { id: 1, code: 'R1', name: 'Region 1', isActive: true };
    regionServiceSpy.getRegions.mockReturnValue(of({ data: [mockRegion] } as any));
    regionServiceSpy.deleteRegion.mockReturnValue(throwError(() => 'Delete error'));
    
    fixture.detectChanges();
    
    component.toggle(mockRegion);
    component.handleConfirmation(true);
    tick();
    
    expect(component.errores()).toEqual(['Delete error']);
  }));

  it('should correctly ask for confirmation and set related properties', fakeAsync(() => {
    const promise = component.askForConfirmation('Test Title', 'Test Text');
    
    expect(component.title).toBe('Test Title');
    expect(component.text).toBe('Test Text');
    expect(component.isConfirming).toBe(true);
    
    component.handleConfirmation(true);
    tick();
    
    promise.then(result => {
      expect(result).toBe(true);
    });
  }));
});
