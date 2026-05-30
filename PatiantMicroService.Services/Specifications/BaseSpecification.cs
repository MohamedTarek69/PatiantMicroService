using PatiantMicroService.Domain.Contracts;
using PatiantMicroService.Domain.Entities;
using System.Linq.Expressions;

namespace PatiantMicroService.Services.Specifications
{
    public abstract class BaseSpecification<TEntity, TKey>
        : ISpecifications<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        #region 🔹 Properties

        public Expression<Func<TEntity, bool>>? Critria { get; protected set; }

        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; private set; }
            = new List<Expression<Func<TEntity, object>>>();

        public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }

        public int Skip { get; protected set; }

        public int Take { get; protected set; }

        public bool IsPaginated { get; protected set; }

        #endregion

        #region 🔹 Constructors

        protected BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Critria = criteria;
        }

        #endregion

        #region 🔹 Methods

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }

        protected void ApplyPagination(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPaginated = true;
        }

        #endregion
    }
}