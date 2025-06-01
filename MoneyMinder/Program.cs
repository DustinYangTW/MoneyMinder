using DailyFileLogger.Extensions;
using Microsoft.OpenApi.Models;

namespace MoneyMinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // �إ� WebApplicationBuilder
            var builder = WebApplication.CreateBuilder(args);

            #region �s�WLogging�ϥΤ覡
            //-------------------------�s�WLogging���U�覡---------------------------
            // �� �[�o�@��A�N��ۭq�ɮ� Logger ���U�i�h
            builder.Logging.ClearProviders();         // �p�G�u�Q�n�ɮ� Log�A�i���M���w�]
            builder.Logging.AddDailyFileLogger();      // �I�s��~���X�R��k
            // �p�G�A�Q�O�d Console/Debug�A�]�i�H�o�ˡG
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            //-------------------------�s�WLogging���U�覡---------------------------
            #endregion

            #region �A�ȵ��U (Service Registration)
            // ���U MVC ����P���ϪA��
            builder.Services.AddControllersWithViews();

            // �ҥκ��I API ���� (�A�Ω� minimal APIs �P Swagger)
            builder.Services.AddEndpointsApiExplorer();

            // ���U�ó]�w Swagger ���ͦ���
            builder.Services.AddSwaggerGen(c =>
            {
                // �w�q Swagger ����T
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyMinder", Version = "v1" });

                // ���J��e�M�ת� XML ������
                var baseXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var baseXmlPath = Path.Combine(AppContext.BaseDirectory, baseXmlFile);
                c.IncludeXmlComments(baseXmlPath, includeControllerXmlComments: true);

                // ���J�B�~���O�w�� XML ���ѡ]�Y���^
                var externalAssemblies = new[] { ""/* ��J�~���ե�W�� */ };
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

            // �إ� Web ���ε{��
            var app = builder.Build();

            #region �����n��޽u (Middleware Pipeline)
            if (!app.Environment.IsDevelopment())
            {
                // �D�}�o���Ҩϥο��~�B�z�����P HSTS
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // �}�o���ұҥ� Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyMinder API V1");
                    c.RoutePrefix = "swagger";    // �� �o���� Swagger UI ���� /swagger
                });
            }

            // �j�� HTTPS
            app.UseHttpsRedirection();

            // �]�w�R�A�ɮתA��
            app.UseStaticFiles();

            // �ҥθ���
            app.UseRouting();

            // ���v (Authorization)
            app.UseAuthorization();

            // �]�w�w�]����
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            #endregion

            // �������ε{��
            app.Run();
        }
    }
}
