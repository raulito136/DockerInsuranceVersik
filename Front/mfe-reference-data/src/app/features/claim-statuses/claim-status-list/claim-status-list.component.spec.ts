import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ClaimStatusListComponent } from './claim-status-list.component';
import { ClaimStatusService } from 'mfe-reference-data/src/app/services/claim-status.service';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ClaimStatus } from 'mfe-reference-data/src/app/models/claim-status.model';

describe('ClaimStatusListComponent', () => {
  let component: ClaimStatusListComponent;
  let fixture: ComponentFixture<ClaimStatusListComponent>;
  let claimServiceSpy: jest.Mocked<ClaimStatusService>;

  beforeEach(async () => {
    const spy = {
      getClaimStatuses: jest.fn(),
      deleteClaimStatus: jest.fn(),
      activateClaimStatus: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ClaimStatusListComponent],
      providers: [
        { provide: ClaimStatusService, useValue: spy },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ClaimStatusListComponent);
    component = fixture.componentInstance;
    claimServiceSpy = TestBed.inject(ClaimStatusService) as unknown as jest.Mocked<ClaimStatusService>;
  });

  it('should create', () => {
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load claim statuses on init', () => {
    const mockClaims: ClaimStatus[] = [
      { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: true },
      { id: 2, code: 'C2', name: 'Claim 2', description: '', isActive: false }
    ];
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: mockClaims } as any));
    
    fixture.detectChanges(); // triggers ngOnInit
    
    expect(claimServiceSpy.getClaimStatuses).toHaveBeenCalledWith(false);
    expect(component.claimsStatuses()).toEqual(mockClaims);
    expect(component.loading()).toBe(false);
  });

  it('should handle error when loading claim statuses', () => {
    claimServiceSpy.getClaimStatuses.mockReturnValue(throwError(() => 'Error loading'));
    
    fixture.detectChanges(); // triggers ngOnInit
    
    expect(component.errores()).toEqual(['Error loading']);
    expect(component.loading()).toBe(false);
  });

  it('should reload data when showAll changes', () => {
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges(); // ngOnInit load
    
    claimServiceSpy.getClaimStatuses.mockClear();
    
    component.onShowAllChange(true);
    
    expect(component.showAll()).toBe(true);
    expect(claimServiceSpy.getClaimStatuses).toHaveBeenCalledWith(true);
  });

  it('should ask for confirmation and delete when toggling an active status', fakeAsync(() => {
    const mockClaim: ClaimStatus = { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: true };
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [mockClaim] } as any));
    claimServiceSpy.deleteClaimStatus.mockReturnValue(of({} as any));
    
    fixture.detectChanges(); // Initial load
    claimServiceSpy.getClaimStatuses.mockClear(); // Reset to track reload after delete
    
    // Call toggle to trigger the confirmation flow
    component.toggle(mockClaim);
    expect(component.isConfirming).toBe(true);
    
    // Simulate user confirming
    component.handleConfirmation(true);
    tick(); // wait for async delete logic to resolve
    
    expect(claimServiceSpy.deleteClaimStatus).toHaveBeenCalledWith(1);
    expect(claimServiceSpy.getClaimStatuses).toHaveBeenCalledWith(false); // Reloads data
  }));

  it('should not delete if confirmation is cancelled', fakeAsync(() => {
    const mockClaim: ClaimStatus = { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: true };
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [mockClaim] } as any));
    
    fixture.detectChanges();
    
    component.toggle(mockClaim);
    component.handleConfirmation(false); // Simulate cancellation
    tick();
    
    expect(claimServiceSpy.deleteClaimStatus).not.toHaveBeenCalled();
  }));

  it('should activate when toggling an inactive status directly without confirmation', () => {
    const mockClaim: ClaimStatus = { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: false };
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [mockClaim] } as any));
    claimServiceSpy.activateClaimStatus.mockReturnValue(of({} as any));
    
    fixture.detectChanges();
    claimServiceSpy.getClaimStatuses.mockClear();
    
    component.toggle(mockClaim);
    
    expect(claimServiceSpy.activateClaimStatus).toHaveBeenCalledWith(1);
    expect(claimServiceSpy.getClaimStatuses).toHaveBeenCalledWith(false); // Reloads data
  });
  
  it('should handle error when activating a status fails', () => {
    const mockClaim: ClaimStatus = { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: false };
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [mockClaim] } as any));
    claimServiceSpy.activateClaimStatus.mockReturnValue(throwError(() => 'Activate error'));
    
    fixture.detectChanges();
    
    component.toggle(mockClaim);
    
    expect(component.errores()).toEqual(['Activate error']);
  });

  it('should handle error when deleting a status fails', fakeAsync(() => {
    const mockClaim: ClaimStatus = { id: 1, code: 'C1', name: 'Claim 1', description: '', isActive: true };
    claimServiceSpy.getClaimStatuses.mockReturnValue(of({ data: [mockClaim] } as any));
    claimServiceSpy.deleteClaimStatus.mockReturnValue(throwError(() => 'Delete error'));
    
    fixture.detectChanges();
    
    component.toggle(mockClaim);
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
