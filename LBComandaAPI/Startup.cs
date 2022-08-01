using LBComandaAPI.Repository.DAO;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LBComandaAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IAdicional, AdicionalDAO>();
            services.AddTransient<IGarcom, GarcomDAO>();
            services.AddTransient<IIngrediente, IngredienteDAO>();
            services.AddTransient<IItemVenda, ItemVendaDAO>();
            services.AddTransient<IMesa, MesaDAO>();
            services.AddTransient<IPontoCarne, PontoCarneDAO>();
            services.AddTransient<IProduto,ProdutoDAO>();
            services.AddTransient<ISabor, SaborDAO>();
            services.AddTransient<IObservacoes, ObservacoesDAO>();
            services.AddTransient<IComanda, ComandaDAO>();
            services.AddTransient<IEntrega, EntregaDAO>();
            services.AddTransient<ICartao, CartaoDAO>();
            services.AddTransient<IRecVenda, RecVendaDAO>();
            services.AddTransient<IItemExcluir, ItemExcluirDAO>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LBComandaAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LBComandaAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
