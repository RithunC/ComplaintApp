import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmpTicketDetailComponent } from './emp-ticket-detail.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';

describe('EmpTicketDetailComponent', () => {
  let f: ComponentFixture<EmpTicketDetailComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpTicketDetailComponent, RouterTestingModule, HttpClientTestingModule],
      providers: [{ provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '42' } } } }]
    }).compileComponents();
    f = TestBed.createComponent(EmpTicketDetailComponent);
  });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('tid resolves from route', () => expect(f.componentInstance.tid).toBe(42));
  it('fmtSize B',  () => expect(f.componentInstance.fmtSize(512)).toBe('512 B'));
  it('fmtSize KB', () => expect(f.componentInstance.fmtSize(2048)).toBe('2.0 KB'));
  it('fmtSize MB', () => expect(f.componentInstance.fmtSize(1048576)).toBe('1.0 MB'));
  it('isClosed false when no ticket', () => expect(f.componentInstance.isClosed).toBeFalse());
  it('isClosed true when closed', () => { f.componentInstance.ticket.set({ statusName: 'Closed' } as any); expect(f.componentInstance.isClosed).toBeTrue(); });
  it('history starts empty', () => expect(f.componentInstance.history()).toEqual([]));
  it('prioBg purple for Urgent', () => expect(f.componentInstance.prioBg('Urgent')).toBe('#7c3aed'));
});
