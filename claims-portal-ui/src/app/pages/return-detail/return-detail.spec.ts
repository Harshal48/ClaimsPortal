import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ActivatedRoute } from '@angular/router';

import { ReturnDetailComponent } from './return-detail';

describe('ReturnDetailComponent', () => {
  let component: ReturnDetailComponent;
  let fixture: ComponentFixture<ReturnDetailComponent>;
  let http: HttpTestingController;

  const testId = '11111111-1111-1111-1111-111111111111';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReturnDetailComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => (key === 'id' ? testId : null),
              },
            },
          },
        },
      ],
    }).compileComponents();

    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(ReturnDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    for (const r of http.match('/api/taxpayers')) {
      r.flush([]);
    }
    for (const r of http.match((req) => req.url.endsWith(`/api/returns/${testId}`))) {
      r.flush({
        id: testId,
        taxpayerId: testId,
        taxYear: 2026,
        returnStatus: 'Draft',
        filingStatus: 'Single',
        reviewerId: null,
        totalIncome: 0,
        totalWithheld: 0,
        totalDeduction: 0,
        createdAtUtc: new Date().toISOString(),
        updatedAtUtc: new Date().toISOString(),
      });
    }

    fixture.detectChanges();
  });

  afterEach(() => {
    http.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
