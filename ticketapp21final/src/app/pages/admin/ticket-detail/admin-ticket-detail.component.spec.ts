import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminTicketDetailComponent } from './admin-ticket-detail.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
describe('AdminTicketDetailComponent', () => {
  let f: ComponentFixture<AdminTicketDetailComponent>;
  beforeEach(async () => { await TestBed.configureTestingModule({ imports:[AdminTicketDetailComponent,RouterTestingModule,HttpClientTestingModule], providers:[{provide:ActivatedRoute,useValue:{snapshot:{paramMap:{get:()=>'1'}}}}] }).compileComponents(); f = TestBed.createComponent(AdminTicketDetailComponent); });
  it('should create', () => expect(f.componentInstance).toBeTruthy());
  it('tid is 1', () => expect(f.componentInstance.tid).toBe(1));
  it('prioBg correct', () => expect(f.componentInstance.prioBg('Urgent')).toBe('#7c3aed'));
});
