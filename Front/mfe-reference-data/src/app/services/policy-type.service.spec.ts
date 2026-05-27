import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { PolicyTypeService } from './policy-type.service';

describe('PolicyTypeService', () => {
  let service: PolicyTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(PolicyTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
