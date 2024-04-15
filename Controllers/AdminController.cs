using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using HeartDisease.Models;
using System.Web;
using CaptchaMvc.HtmlHelpers;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System;
using System.Web.Security;
using System.Net;

namespace HeartDisease.Controllers
{
    
    public class AdminController : Controller
    {

        HDEntities db = new HDEntities();

        //Admin ControlePanel

        public ActionResult AdminIndex()
        {
           
            if (Session["idUser"] != null )
            {
               
                    return View();

            }

            return RedirectToAction("AdminLogin");

        }
        //--------------------------------------------------Admin Register and login-------------------------------------------------


        //GET: Register
        
        public ActionResult AdminRegister()
        {
 
            return View();
       
        }

        //POST: Register
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminRegister(User _user)
        {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.ErrorMessage = "Invalid Captcha";
                        return View("AdminRegister", _user);
                    }

                    _user.Password = GetMD5(_user.Password);
             
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("AdminLogin");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }


            }
            return View();


        }

        public ActionResult AdminLogin()
        {
            
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(string email, string password)
        {

            var f_password = GetMD5(password);
            var data = db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
            if (data.Count() > 0)
            {
                switch (data.FirstOrDefault().RoleName)
                {
                    case "Visitor":
                        ViewBag.error = "Access Denied !!";
                        return View();
                    case "Admin":
                        if (!this.IsCaptchaValid(""))
                        {
                            ViewBag.ErrorMessage = "Invalid Captcha";
                            return View();
                        }

                        //add session
                        Session["UserName"] = data.FirstOrDefault().UserName;
                        Session["Email"] = data.FirstOrDefault().Email;
                        Session["idUser"] = data.FirstOrDefault().idUser;
                        return RedirectToAction("AdminIndex");
                    case "Manager":
                        if (!this.IsCaptchaValid(""))
                        {
                            ViewBag.ErrorMessage = "Invalid Captcha";
                            return View();
                        }

                        //add session
                        Session["UserName"] = data.FirstOrDefault().UserName;
                        Session["Email"] = data.FirstOrDefault().Email;
                        Session["idUser"] = data.FirstOrDefault().idUser;
                        return RedirectToAction("AdminIndex");
                }
            }

            ViewBag.error = "Login failed";
            return View();


        }


        //Logout
        public ActionResult AdminLogout()
        {
            Session.Clear();//remove session
            return RedirectToAction("AdminLogin");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        //--------------------------------------------------------------Roles------------------------------------------------------------------------
        // GET: Roles
        
        public async Task<ActionResult> RoleIndex()
        {
            if (Session["idUser"] != null)
            {
                return View(await db.Roles.ToListAsync());
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        // GET: Roles/Details/5
        
        public async Task<ActionResult> RoleDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await db.Roles.FindAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // GET: Roles/Create
        
        public ActionResult RoleCreate()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleCreate([Bind(Include = "Id,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                await db.SaveChangesAsync();
                return RedirectToAction("RoleIndex");
            }

            return View(role);
        }

        // GET: Roles/Edit/5
        
        public async Task<ActionResult> RoleEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await db.Roles.FindAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleEdit([Bind(Include = "Id,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: Roles/Delete/5
        
        public async Task<ActionResult> RoleDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await db.Roles.FindAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        
        [HttpPost, ActionName("RoleDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleDeleteConfirmed(string id)
        {
            Role role = await db.Roles.FindAsync(id);
            db.Roles.Remove(role);
            await db.SaveChangesAsync();
            return RedirectToAction("RoleIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //--------------------------------------------------------------Users------------------------------------------------------------------------

        // GET: Admin
        
        public async Task<ActionResult> UserIndex()
        {
            if (Session["idUser"] != null)
            {
                return View(await db.Users.ToListAsync());
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }

          
        }

        // GET: Admin/Details/5
        
        public async Task<ActionResult> UserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Create
        
        public ActionResult UserCreate()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserCreate([Bind(Include = "idUser,UserName,LastName,Gender,Age,Address,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.RoleName = "Visitor";
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("UserIndex");
            }

            return View(user);
        }

        // GET: Admin/Edit/5
        
        public async Task<ActionResult> UserEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
               
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserEdit([Bind(Include = "idUser,UserName,LastName,Gender,Age,Address,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("UserIndex");
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        
        public async Task<ActionResult> UserDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Delete/5
        
        [HttpPost, ActionName(" UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserDeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("UserIndex");
        }

        // --------------------------------------------------Subspecialties----------------------------------------------------------
        // GET: Subspecialties
        
        public async Task<ActionResult> SubspecIndex()
        {
            
                return View(await db.Subspecialties.ToListAsync());
           
         
        }

        // GET: Subspecialties/Details/5
        
        public async Task<ActionResult> SubspecDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subspecialty S = await db.Subspecialties.FindAsync(id);
            if (S == null)
            {
                return HttpNotFound();
            }
            return View(S);
        }
        //============================================================================= 
        // GET: Subspecialties/Create
        

        public ActionResult SubspecCreate()
        {
            Subspecialty s = new Subspecialty();
            return View(s);
        }


        // Subspecialties/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubspecCreate(Subspecialty Sub)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["image"];
                if (file != null)
                {
                    Sub.Photo = new byte[file.ContentLength];
                    file.InputStream.Read(Sub.Photo, 0, file.ContentLength);
                }
                db.Subspecialties.Add(Sub);
                db.SaveChanges();

                //Redirect to Index Action.
                return RedirectToAction("SubspecIndex");
            }
 
            return View(Sub);
        }
        // ==================================================================================



        // GET: Subspecialties/Edit/5
        
        public async Task<ActionResult> SubspecEdit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subspecialty s = await db.Subspecialties.FindAsync(id);
            if (s == null)
            {
                return HttpNotFound();
            }
            HttpPostedFileBase file = Request.Files["image"];
            
                s.Photo = new byte[file.ContentLength];
                file.InputStream.Read(s.Photo, 0, file.ContentLength);
               
            return View(s);

        }

        // POST: Subspecialties/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubspecEdit([Bind(Include = "Id,Title,Photo")] Subspecialty s)
        {
            if (ModelState.IsValid)
            {
                db.Entry(s).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SubspecIndex");
            }
            return View(s);
        }

        // GET: Subspecialties/Delete/5
        
        public async Task<ActionResult> SubspecDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subspecialty s = await db.Subspecialties.FindAsync(id);
            if (s == null)
            {
                return HttpNotFound();
            }
            return View(s);
        }

        // POST: Subspecialties/Delete/5
        
        [HttpPost, ActionName("SubspecDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubspecDeleteConfirmed(int id)
        {
            Subspecialty s = await db.Subspecialties.FindAsync(id);
            db.Subspecialties.Remove(s);
            await db.SaveChangesAsync();
            return RedirectToAction("SubspecIndex");
        }

        // --------------------------------------------------MedicalTopics----------------------------------------------------------
        // GET: MedicalTopics
        
        public async Task<ActionResult> TopicIndex()
        {
            var medicalTopics = db.MedicalTopics.Include(m => m.Subspecialty).Include(m => m.User);
     
            return View(await medicalTopics.ToListAsync());
        }

        // GET: MedicalTopics/Details/5
        
        public async Task<ActionResult> TopicDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalTopic medicalTopic = await db.MedicalTopics.FindAsync(id);
            if (medicalTopic == null)
            {
                return HttpNotFound();
            }
            return View(medicalTopic);
        }
        //============================================================================= 
        
        public ActionResult TopicCreate()
        {
            MedicalTopic t = new MedicalTopic();
            ViewBag.idSubspec = new SelectList(db.Subspecialties, "Id", "Title");
            var id = (int?)Session["idUser"];
            var user = db.Users.Where(s => s.idUser ==id).ToList();
            ViewBag.idUser = new SelectList(user, "idUser", "UserName");
            return View(t);
        }


        // MedicalTopics/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TopicCreate(MedicalTopic medicalTopic)
        {
            if (ModelState.IsValid)
            {
                
                HttpPostedFileBase file = Request.Files["image"];
                if (file != null)
                {
                    medicalTopic.Photo = new byte[file.ContentLength];
                    file.InputStream.Read(medicalTopic.Photo,0,file.ContentLength);
                }
                db.MedicalTopics.Add(medicalTopic);
                db.SaveChanges();

                //Redirect to Index Action.
                return RedirectToAction("TopicIndex");
            }

            ViewBag.idSubspec = new SelectList(db.Subspecialties, "Id", "Title" ,medicalTopic.idSubspec);
            ViewBag.idUser = (int?)Session["idUser"];
            return View(medicalTopic);
        }
        // ==================================================================================
        // GET: MedicalTopics/Edit/5
        
        public async Task<ActionResult> TopicEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalTopic medicalTopic = await db.MedicalTopics.FindAsync(id);
            if (medicalTopic == null)
            {
                return HttpNotFound();
            }
            ViewBag.idSubspec = new SelectList(db.Subspecialties, "Id", "Title", medicalTopic.idSubspec);
            ViewBag.idUser = (int?)Session["idUser"];
            return View(medicalTopic);
        }

        // POST: MedicalTopics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TopicEdit([Bind(Include = "Id,Title,C_Content_,Photo,C_File_,Views,date,C_References_,idUser,idSubspec")] MedicalTopic medicalTopic)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(medicalTopic).State = EntityState.Modified;
 
                HttpPostedFileBase file = Request.Files["image"];
                if (file != null)
                {
                    medicalTopic.Photo = new byte[file.ContentLength];
                    file.InputStream.Read(medicalTopic.Photo, 0, file.ContentLength);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("TopicIndex");
            }
            ViewBag.idSubspec = new SelectList(db.Subspecialties, "Id", "Title", medicalTopic.idSubspec);
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", medicalTopic.idUser);
            return View(medicalTopic);
        }

        // GET: MedicalTopics/Delete/5
        
        public async Task<ActionResult> TopicDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalTopic medicalTopic = await db.MedicalTopics.FindAsync(id);
            if (medicalTopic == null)
            {
                return HttpNotFound();
            }
            return View(medicalTopic);
        }

        // POST: MedicalTopics/Delete/5
        
        [HttpPost, ActionName("TopicDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TopicDeleteConfirmed(int id)
        {
            MedicalTopic medicalTopic = await db.MedicalTopics.FindAsync(id);
            db.MedicalTopics.Remove(medicalTopic);
            await db.SaveChangesAsync();
            return RedirectToAction("TopicIndex");
        }

        //-------------------------------------------------diet and food for heart patients----------------------------------------------------------------
        // GET: food
        public async Task<ActionResult> DietFoodIndex()
        {
            var diet_and_food_for_heart_patients = db.diet_and_food_for_heart_patients.Include(d => d.User);
            return View(await diet_and_food_for_heart_patients.ToListAsync());
        }

        // GET: food/Details/5
        public async Task<ActionResult> DietFoodDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diet_and_food_for_heart_patient diet_and_food_for_heart_patient = await db.diet_and_food_for_heart_patients.FindAsync(id);
            if (diet_and_food_for_heart_patient == null)
            {
                return HttpNotFound();
            }
            return View(diet_and_food_for_heart_patient);
        }

        // GET: food/Create
        public ActionResult DietFoodCreate()
        {
            diet_and_food_for_heart_patient t = new diet_and_food_for_heart_patient();
            var id = (int?)Session["idUser"];
            var user = db.Users.Where(s => s.idUser == id).ToList();
            ViewBag.idUser = new SelectList(user, "idUser", "UserName");
            return View(t);
        }
 

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DietFoodCreate( diet_and_food_for_heart_patient diet)
        {
            if (ModelState.IsValid)
            {

                HttpPostedFileBase file = Request.Files["image"];
                if (file != null)
                {
                    diet.Photo = new byte[file.ContentLength];
                    file.InputStream.Read(diet.Photo, 0, file.ContentLength);
                }
                db.diet_and_food_for_heart_patients.Add(diet);
                db.SaveChanges();

                //Redirect to Index Action.
                return RedirectToAction("DietFoodIndex");
            }

             
            ViewBag.idUser = (int?)Session["idUser"];
            return View(diet);
        }


        // GET: food/Edit/5
        public async Task<ActionResult> DietFoodEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diet_and_food_for_heart_patient diet_and_food_for_heart_patient = await db.diet_and_food_for_heart_patients.FindAsync(id);
            if (diet_and_food_for_heart_patient == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUser = (int?)Session["idUser"];
            return View(diet_and_food_for_heart_patient);
        }

        // POST: food/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DietFoodEdit([Bind(Include = "Id,Title,C_Content_,Photo,C_File_,Views,date,idUser,C_References_")] diet_and_food_for_heart_patient diet_and_food_for_heart_patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diet_and_food_for_heart_patient).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("DietFoodIndex");
            }
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", diet_and_food_for_heart_patient.idUser);
            return View(diet_and_food_for_heart_patient);
        }

        // GET: food/Delete/5
        public async Task<ActionResult> DietFoodDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diet_and_food_for_heart_patient diet_and_food_for_heart_patient = await db.diet_and_food_for_heart_patients.FindAsync(id);
            if (diet_and_food_for_heart_patient == null)
            {
                return HttpNotFound();
            }
            return View(diet_and_food_for_heart_patient);
        }

        // POST: food/Delete/5
        [HttpPost, ActionName("DietFoodDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DietFoodDeleteConfirmed(int id)
        {
            diet_and_food_for_heart_patient diet_and_food_for_heart_patient = await db.diet_and_food_for_heart_patients.FindAsync(id);
            db.diet_and_food_for_heart_patients.Remove(diet_and_food_for_heart_patient);
            await db.SaveChangesAsync();
            return RedirectToAction("DietFoodIndex");
        }
        //------------------------------------------Heart disease and symptoms------------------------------------------------------------------------

        // GET: Heart_disease_and_symptom
        public async Task<ActionResult> HDiseaseIndex()
        {
            var heart_disease_and_symptoms = db.Heart_disease_and_symptoms.Include(h => h.User);
            return View(await heart_disease_and_symptoms.ToListAsync());
        }

        // GET: Heart_disease_and_symptom/Details/5
        public async Task<ActionResult> HDiseaseDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heart_disease_and_symptom heart_disease_and_symptom = await db.Heart_disease_and_symptoms.FindAsync(id);
            if (heart_disease_and_symptom == null)
            {
                return HttpNotFound();
            }
            return View(heart_disease_and_symptom);
        }

        // GET: Heart_disease_and_symptom/Create
        public ActionResult HDiseaseCreate()
        {
            Heart_disease_and_symptom hd = new Heart_disease_and_symptom();
            ViewBag.id = new SelectList(db.Heart_disease_and_symptoms, "Id", "Title");
            var id = (int?)Session["idUser"];
            var user = db.Users.Where(s => s.idUser == id).ToList();
            ViewBag.idUser = new SelectList(user, "idUser", "UserName");
            return View(hd);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HDiseaseCreate( Heart_disease_and_symptom hd)
        {
            if (ModelState.IsValid)
            {

                HttpPostedFileBase file = Request.Files["image"];
                if (file != null)
                {
                    hd.Photo = new byte[file.ContentLength];
                    file.InputStream.Read(hd.Photo, 0, file.ContentLength);
                }
                db.Heart_disease_and_symptoms.Add(hd);
                db.SaveChanges();

                //Redirect to Index Action.
                return RedirectToAction("HDiseaseIndex");
            }
            ViewBag.idUser = (int?)Session["idUser"];
            return View(hd);
        }

      

        // GET: Heart_disease_and_symptom/Edit/5
        public async Task<ActionResult> HDiseaseEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heart_disease_and_symptom heart_disease_and_symptom = await db.Heart_disease_and_symptoms.FindAsync(id);
            if (heart_disease_and_symptom == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUser = (int?)Session["idUser"];
            
            return View(heart_disease_and_symptom);
        }

        // POST: Heart_disease_and_symptom/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HDiseaseEdit([Bind(Include = "Id,Title,C_Content_,Photo,C_File_,Views,date,idUser,C_References_")] Heart_disease_and_symptom heart_disease_and_symptom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(heart_disease_and_symptom).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("HDiseaseIndex");
            }
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", heart_disease_and_symptom.idUser);
            return View(heart_disease_and_symptom);
        }

        // GET: Heart_disease_and_symptom/Delete/5
        public async Task<ActionResult> HDiseaseDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heart_disease_and_symptom heart_disease_and_symptom = await db.Heart_disease_and_symptoms.FindAsync(id);
            if (heart_disease_and_symptom == null)
            {
                return HttpNotFound();
            }
            return View(heart_disease_and_symptom);
        }

        // POST: Heart_disease_and_symptom/Delete/5
        [HttpPost, ActionName("HDiseaseDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HDiseaseDeleteConfirmed(int id)
        {
            Heart_disease_and_symptom heart_disease_and_symptom = await db.Heart_disease_and_symptoms.FindAsync(id);
            db.Heart_disease_and_symptoms.Remove(heart_disease_and_symptom);
            await db.SaveChangesAsync();
            return RedirectToAction("HDiseaseIndex");
        }
        //--------------------------------------------------------------------------consulting-------------------------------------------------------
        // GET: Consultings not replaied

        public async Task<ActionResult> ConsultingIndex()
        {
            var consultings = db.Consultings.Include(c => c.User).Where(s => s.Answer == null).OrderBy(s => s.date);
            return View(await consultings.ToListAsync());
        }

        // GET: Consultings replaied
        public async Task<ActionResult> ConsultingIndex2()
        {
            var consultings = db.Consultings.Include(c => c.User).Where(s => s.Answer != null).OrderBy(s => s.date);
            return View(await consultings.ToListAsync());
        }

        // GET: Consultings/Details/5

        public async Task<ActionResult> ConsultingDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulting consulting = await db.Consultings.FindAsync(id);
            if (consulting == null)
            {
                return HttpNotFound();
            }
            return View(consulting);
        }

        // GET: Consultings/Create
      
        public ActionResult ConsultingCreate()
        {
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName");
            return View();
        }

        // POST: Consultings/Create
      
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConsultingCreate([Bind(Include = "Id,Title,C_Patient_age_,C_Patient_Gender_,C_Content_,Answer,date,idUser")] Consulting consulting)
        {
            if (ModelState.IsValid)
            {
                db.Consultings.Add(consulting);
                await db.SaveChangesAsync();
                return RedirectToAction("ConsultingIndex");
            }

            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", consulting.idUser);
            return View(consulting);
        }

        // GET: Consultings/Edit/5
      
        public async Task<ActionResult> ConsultingEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulting consulting = await db.Consultings.FindAsync(id);
            if (consulting == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", consulting.idUser);
            return View(consulting);
        }

        // POST: Consultings/Edit/5
      
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConsultingEdit([Bind(Include = "Id,Title,C_Patient_age_,C_Patient_Gender_,C_Content_,Answer,date,idUser")] Consulting consulting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consulting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("ConsultingIndex");
            }
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", consulting.idUser);
            return View(consulting);
        }

        // GET: Consultings/Delete/5
      
        public async Task<ActionResult> ConsultingDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulting consulting = await db.Consultings.FindAsync(id);
            if (consulting == null)
            {
                return HttpNotFound();
            }
            return View(consulting);
        }

        // POST: Consultings/Delete/5
      
        [HttpPost, ActionName("ConsultingDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConsultingDeleteConfirmed(int id)
        {
            Consulting consulting = await db.Consultings.FindAsync(id);
            db.Consultings.Remove(consulting);
            await db.SaveChangesAsync();
            return RedirectToAction("ConsultingIndex");
        }

    }
}
