import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { ClaimStatusService } from './claim-status.service';

describe('ClaimStatusService', () => {
  let service: ClaimStatusService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ClaimStatusService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
