import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminTicketsComponent } from './admin-tickets.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AdminTicketsComponent', () => {
  let f: ComponentFixture<AdminTicketsComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AdminTicketsComponent,RouterTestingModule,HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(AdminTicketsComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('prioBg red for High', () => expect(f.componentInstance.prioBg('High')).toBe('#dc2626'));
  it('isOverdue true for past date', () => expect(f.componentInstance.isOverdue('2000-01-01')).toBeTrue());
});
