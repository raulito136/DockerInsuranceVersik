import { TestBed } from '@angular/core/testing';

import { PolicyHolderService } from './policy-holder.service';

describe('PolicyHolderService', () => {
  let service: PolicyHolderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PolicyHolderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
