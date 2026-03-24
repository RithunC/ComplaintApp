import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('AdminDashboardComponent', () => {
  let f: ComponentFixture<AdminDashboardComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AdminDashboardComponent,RouterTestingModule,HttpClientTestingModule] }).compileComponents(); f = TestBed.createComponent(AdminDashboardComponent); f.detectChanges(); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('pct returns 0 when total 0', () => expect(f.componentInstance.pct(5,0)).toBe(0));
  it('pct returns 50', () => expect(f.componentInstance.pct(5,10)).toBe(50));
});
