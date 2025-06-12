using DailyFileLogger.Extensions;
using Microsoft.OpenApi.Models;

namespace MoneyMinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register custom services
            builder.Services.AddTransient<MoneyMinder.Services.ILogService, MoneyMinder.Services.LogService>();
            // 建立 WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);

            #region 新增Logging使用方式
            //-------------------------新增Logging註冊方式---------------------------
            // ← 加這一行，就把自訂檔案 Logger 註冊進去
            builder.Logging.ClearProviders();         // 如果只想要檔案 Log，可先清掉預設
            builder.Logging.AddDailyFileLogger();      // 呼叫剛才的擴充方法
            // 如果你想保留 Console/Debug，也可以這樣：
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            //-------------------------新增Logging註冊方式---------------------------
            #endregion

            #region 服務註冊 (Service Registration)
            // 註冊 MVC 控制器與視圖服務
            builder.Services.AddControllersWithViews();

            // 啟用端點 API 探索 (適用於 minimal APIs 與 Swagger)
            builder.Services.AddEndpointsApiExplorer();

            // 註冊並設定 Swagger 文件生成器
            builder.Services.AddSwaggerGen(c =>
            {
                // 定義 Swagger 文件資訊
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyMinder", Version = "v1" });

                // 載入當前專案的 XML 註解檔
                var baseXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var baseXmlPath = Path.Combine(AppContext.BaseDirectory, baseXmlFile);
                c.IncludeXmlComments(baseXmlPath, includeControllerXmlComments: true);

                // 載入額外類別庫的 XML 註解（若有）
                var externalAssemblies = new[] { ""/* 填入外部組件名稱 */ };
                foreach (var assemblyName in externalAssemblies)
                {
                    var xmlFile = $"{assemblyName}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            });
            #endregion

            // 建立 Web 應用程式
            var app = builder.Build();

            #region 中介軟體管線 (Middleware Pipeline)
            if (!app.Environment.IsDevelopment())
            {
                // 非開發環境使用錯誤處理頁面與 HSTS
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // 開發環境啟用 Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyMinder API V1");
                    c.RoutePrefix = "swagger";    // ← 這行讓 Swagger UI 掛到 /swagger
                });
            }

            // 強制 HTTPS
            app.UseHttpsRedirection();

            // 設定靜態檔案服務
            app.UseStaticFiles();

            // 啟用路由
            app.UseRouting();

            // 授權 (Authorization)
            app.UseAuthorization();

            // 設定預設路由
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            #endregion

            // 執行應用程式
            app.Run();
        }
    }
}
