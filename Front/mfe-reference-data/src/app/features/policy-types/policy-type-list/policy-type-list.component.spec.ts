import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PolicyTypeListComponent } from './policy-type-list.component';
import { PolicyTypeService } from 'mfe-reference-data/src/app/services/policy-type.service';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PolicyType } from 'mfe-reference-data/src/app/models/policy-type.model';

describe('PolicyTypeListComponent', () => {
  let component: PolicyTypeListComponent;
  let fixture: ComponentFixture<PolicyTypeListComponent>;
  let policyServiceSpy: jest.Mocked<PolicyTypeService>;

  beforeEach(async () => {
    const spy = {
      getPolicyTypes: jest.fn(),
      deletePolicyType: jest.fn(),
      activatePolicyType: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [PolicyTypeListComponent],
      providers: [
        { provide: PolicyTypeService, useValue: spy },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PolicyTypeListComponent);
    component = fixture.componentInstance;
    policyServiceSpy = TestBed.inject(PolicyTypeService) as unknown as jest.Mocked<PolicyTypeService>;
  });

  it('should create', () => {
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load policy types on init', () => {
    const mockPolicies: PolicyType[] = [
      { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: true },
      { id: 2, code: 'P2', name: 'Policy 2', description: '', isActive: false }
    ];
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: mockPolicies } as any));
    
    fixture.detectChanges();
    
    expect(policyServiceSpy.getPolicyTypes).toHaveBeenCalledWith(false);
    expect(component.policyTypes()).toEqual(mockPolicies);
    expect(component.loading()).toBe(false);
  });

  it('should handle error when loading policy types', () => {
    policyServiceSpy.getPolicyTypes.mockReturnValue(throwError(() => 'Error loading'));
    
    fixture.detectChanges();
    
    expect(component.errores()).toEqual(['Error loading']);
    expect(component.loading()).toBe(false);
  });

  it('should reload data when showAll changes', () => {
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [] } as any));
    fixture.detectChanges(); // ngOnInit load
    
    policyServiceSpy.getPolicyTypes.mockClear();
    
    component.onShowAllChange(true);
    
    expect(component.showAll()).toBe(true);
    expect(policyServiceSpy.getPolicyTypes).toHaveBeenCalledWith(true);
  });

  it('should ask for confirmation and delete when toggling an active policy type', fakeAsync(() => {
    const mockPolicy: PolicyType = { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: true };
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [mockPolicy] } as any));
    policyServiceSpy.deletePolicyType.mockReturnValue(of({} as any));
    
    fixture.detectChanges();
    policyServiceSpy.getPolicyTypes.mockClear();
    
    component.toggle(mockPolicy);
    expect(component.isConfirming).toBe(true);
    
    component.handleConfirmation(true);
    tick();
    
    expect(policyServiceSpy.deletePolicyType).toHaveBeenCalledWith(1);
    expect(policyServiceSpy.getPolicyTypes).toHaveBeenCalledWith(false); 
  }));

  it('should not delete if confirmation is cancelled', fakeAsync(() => {
    const mockPolicy: PolicyType = { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: true };
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [mockPolicy] } as any));
    
    fixture.detectChanges();
    
    component.toggle(mockPolicy);
    component.handleConfirmation(false);
    tick();
    
    expect(policyServiceSpy.deletePolicyType).not.toHaveBeenCalled();
  }));

  it('should activate when toggling an inactive policy type directly without confirmation', () => {
    const mockPolicy: PolicyType = { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: false };
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [mockPolicy] } as any));
    policyServiceSpy.activatePolicyType.mockReturnValue(of({} as any));
    
    fixture.detectChanges();
    policyServiceSpy.getPolicyTypes.mockClear();
    
    component.toggle(mockPolicy);
    
    expect(policyServiceSpy.activatePolicyType).toHaveBeenCalledWith(1);
    expect(policyServiceSpy.getPolicyTypes).toHaveBeenCalledWith(false);
  });
  
  it('should handle error when activating a policy type fails', () => {
    const mockPolicy: PolicyType = { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: false };
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [mockPolicy] } as any));
    policyServiceSpy.activatePolicyType.mockReturnValue(throwError(() => 'Activate error'));
    
    fixture.detectChanges();
    
    component.toggle(mockPolicy);
    
    expect(component.errores()).toEqual(['Activate error']);
  });

  it('should handle error when deleting a policy type fails', fakeAsync(() => {
    const mockPolicy: PolicyType = { id: 1, code: 'P1', name: 'Policy 1', description: '', isActive: true };
    policyServiceSpy.getPolicyTypes.mockReturnValue(of({ data: [mockPolicy] } as any));
    policyServiceSpy.deletePolicyType.mockReturnValue(throwError(() => 'Delete error'));
    
    fixture.detectChanges();
    
    component.toggle(mockPolicy);
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
