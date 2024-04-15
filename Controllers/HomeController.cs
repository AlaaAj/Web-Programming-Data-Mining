using CaptchaMvc.HtmlHelpers;
using HeartDisease.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace HeartDisease.Controllers
{
    public class HomeController : Controller
    {
        
         HDEntities db = new HDEntities();
        // GET: Home
        public ActionResult Index()
        {
            
          
            ViewBag.Images = db.MedicalTopics.OrderByDescending(c => c.Views).ToList();
            ViewBag.photos = db.Subspecialties.ToList();
            return View();
        }


        //GET: Register

        public ActionResult Register()
    {
        return View();
    }

    //POST: Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(User _user)
    {
        if (ModelState.IsValid)
        {
            var check = db.Users.FirstOrDefault(s => s.Email == _user.Email);
            if (check == null)
            {
                    if (!this.IsCaptchaValid(""))
                    {
                        ViewBag.ErrorMessage = "Invalid Captcha";
                        return View("Register", _user);
                    }

                    _user.Password = GetMD5(_user.Password);
                    _user.RoleName = "Visitor";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Users.Add(_user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.error = "Email already exists";
                return View();
            }


        }
        return View();


    }

    public ActionResult Login()
    {
        return View();
    }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            
                var f_password = GetMD5(password);
                var data = db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
            if (data.Count() > 0)
            {
                switch (data.FirstOrDefault().RoleName)
                {
                    case "Visitor":
                        if (!this.IsCaptchaValid(""))
                        {
                            ViewBag.ErrorMessage = "Invalid Captcha";
                            return View();
                        }

                        //add session
                        Session["UserName"] = data.FirstOrDefault().UserName;
                        Session["Email"] = data.FirstOrDefault().Email;
                        Session["idUser"] = data.FirstOrDefault().idUser;
                        return RedirectToAction("Index");
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
        public ActionResult Logout()
    {
        Session.Clear();//remove session
        return RedirectToAction("Login");
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

        public ActionResult TDetails(int? id)
        {
            MedicalTopic topic = (from c in db.MedicalTopics
                                        where c.Id == id
                                        select c).FirstOrDefault();


            topic.Views = topic.Views+=1;
             
                db.SaveChanges();
                 
            return View(topic);
        }

        // GET: Topics
        public ActionResult Topic()
        {
            ViewBag.Images = db.MedicalTopics.OrderByDescending(c => c.Views).ToList();    
            return View();
        }

        // GET: Diet
        public ActionResult Diet()
        {

            ViewBag.Images = db.diet_and_food_for_heart_patients.ToList();

            return View();
        }

        // GET: HD
        public ActionResult HD()
        {

            ViewBag.Images = db.Heart_disease_and_symptoms.ToList();

            return View();
        }

        //------------------------------------------------------------------------------user consulting-------------------------------------------------------------------------------
        // GET: Consultings

        public async Task<ActionResult> ConIndex()
        {
            if (Session["idUser"] != null)
            {
                int userId = (int)Session["idUser"];
                var consultings = db.Consultings.Include(c => c.User);

                var list = consultings.Where(x => x.idUser == userId);
                return View(await list.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login");
            }

         
        }

        // GET: Consultings/Details/5
        
        public async Task<ActionResult> ConDetails(int? id)
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
        
        public ActionResult ConCreate()
        {
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName");
            return View();
        }

        // POST: Consultings/Create
        
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConCreate([Bind(Include = "Id,Title,C_Patient_age_,C_Patient_Gender_,C_Content_,Answer,date,idUser")] Consulting consulting)
        {
            if (ModelState.IsValid)
            {
                consulting.idUser = (int?)Session["idUser"];
                db.Consultings.Add(consulting);
                await db.SaveChangesAsync();
                return RedirectToAction("ConIndex");
            }

            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", consulting.idUser);
            return View(consulting);
        }

        // GET: Consultings/Edit/5
        
        public async Task<ActionResult> ConEdit(int? id)
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
        public async Task<ActionResult> ConEdit([Bind(Include = "Id,Title,C_Patient_age_,C_Patient_Gender_,C_Content_,Answer,date,idUser")] Consulting consulting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consulting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.idUser = new SelectList(db.Users, "idUser", "UserName", consulting.idUser);
            return View(consulting);
        }

        // GET: Consultings/Delete/5
        
        public async Task<ActionResult> ConDelete(int? id)
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
        
        [HttpPost, ActionName("ConDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConDeleteConfirmed(int id)
        {
            Consulting consulting = await db.Consultings.FindAsync(id);
            db.Consultings.Remove(consulting);
            await db.SaveChangesAsync();
            return RedirectToAction("ConIndex");
        }


        //-----------------------------------------------------------------------------------Admin---------------------------------------------------------------------------
        public ActionResult AdminIndex()
        {

            if (Session["idUser"] != null)
            {

                return View();

            }

            return RedirectToAction("AdminLogin");

        }

        //---------------- DATAMINING---------------
        
        public ActionResult ExpertIndex(value PATIENT_VALUE, string calculate)
        {
            if (Session["idUser"] != null)
            {
                Id3 id3 = new Id3();
                class_bayes bayes =new class_bayes();

                PATIENT_VALUE = id3.classification(PATIENT_VALUE);
                //if (calculate == "id3")
                //{
                //    PATIENT_VALUE = id3.classification(PATIENT_VALUE);

                //}
                //if (calculate == "bayes")
                //{

                //     /// PATIENT_VALUE.result= bayes.classification(PATIENT_VALUE);


                //}


                return View(PATIENT_VALUE);
            }
            else
            {
                return RedirectToAction("Login");
            }

           
        }

       
        public ActionResult test()
        {
            //Id3 id3 = new Id3();

            //   PATIENT_VALUE = id3.classification(PATIENT_VALUE);

            return View();
        }

    }
}