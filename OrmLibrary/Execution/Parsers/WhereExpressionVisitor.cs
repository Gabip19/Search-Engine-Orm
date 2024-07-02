using System.Collections;
using System.Linq.Expressions;
using OrmLibrary.Execution.Query;
using OrmLibrary.Extensions;

namespace OrmLibrary.Execution.Parsers;

public class WhereExpressionVisitor : ExpressionVisitor
{
    private readonly List<WhereConditionDetails> _conditions = new();
    private WhereConditionDetails _currentComparison;
    private int _currentGroupLevel;

    public List<WhereConditionDetails> Conditions => _conditions;

    public override Expression Visit(Expression node)
    {
        if (node == null) return null;
        return base.Visit(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        bool isLogicalOperator = node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse;

        if (isLogicalOperator)
        {
            _currentGroupLevel++;
        }

        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.AndAlso:
                _currentComparison = new WhereConditionDetails { LogicalOperator = "AND", GroupLevel = _currentGroupLevel };
                _conditions.Add(_currentComparison);
                break;
            case ExpressionType.OrElse:
                _currentComparison = new WhereConditionDetails { LogicalOperator = "OR", GroupLevel = _currentGroupLevel };
                _conditions.Add(_currentComparison);
                break;
            case ExpressionType.Equal:
                _currentComparison.Operation = "=";
                break;
            case ExpressionType.NotEqual:
                _currentComparison.Operation = "!=";
                break;
            case ExpressionType.GreaterThan:
                _currentComparison.Operation = ">";
                break;
            case ExpressionType.GreaterThanOrEqual:
                _currentComparison.Operation = ">=";
                break;
            case ExpressionType.LessThan:
                _currentComparison.Operation = "<";
                break;
            case ExpressionType.LessThanOrEqual:
                _currentComparison.Operation = "<=";
                break;
            default:
                throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
        }

        Visit(node.Right);

        if (isLogicalOperator)
        {
            _currentGroupLevel--;
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.MemberType == System.Reflection.MemberTypes.Property)
        {
            if (_currentComparison == null || _currentComparison.Operation == null)
            {
                _currentComparison = new WhereConditionDetails { GroupLevel = _currentGroupLevel };
                _conditions.Add(_currentComparison);
            }

            if (node.Member.Name == "Length" && node.Expression is MemberExpression memberExpression)
            {
                _currentComparison.PropertyName = $"{memberExpression.Member.Name}.Length";
            }
            else
            {
                _currentComparison.PropertyName = node.Member.Name;
            }
        }
        else if (node.Member.MemberType == System.Reflection.MemberTypes.Field)
        {
            var lambda = Expression.Lambda(node);
            var value = lambda.Compile().DynamicInvoke();
            _currentComparison.Value = value;
            _currentComparison.IsConstant = true;
            _currentComparison.ValueType = node.Type;
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (_currentComparison != null)
        {
            _currentComparison.Value = node.Value;
            _currentComparison.IsConstant = true;
            _currentComparison.ValueType = node.Type;
        }

        return node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_currentComparison != null)
        {
            _currentComparison.IsConstant = false;
            _currentComparison.ValueType = node.Type;
        }
        return base.VisitParameter(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "StartsWith":
                HandleStringMethod(node, "LIKE", "", "%");
                break;
            case "EndsWith":
                HandleStringMethod(node, "LIKE", "%");
                break;
            case "Contains":
                if (node.Method.DeclaringType == typeof(string))
                {
                    HandleStringMethod(node, "LIKE", "%", "%");
                }
                else if (node.Method.DeclaringType.IsGenericType && node.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    HandleCollectionContainsMethod(node);
                }
                else if (node.Method.DeclaringType == typeof(Enumerable))
                {
                    HandleCollectionContainsMethod(node);
                }
                break;
            case "Equals":
                HandleEqualsMethod(node);
                break;
            case "IsNullOrEmpty":
                HandleIsNullOrEmptyMethod(node);
                break;
            case "IsNullOrWhiteSpace":
                HandleIsNullOrWhiteSpaceMethod(node);
                break;
            default:
                throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
        }

        return node;
    }

    private void HandleStringMethod(MethodCallExpression node, string sqlOperator, string prefix = "", string suffix = "")
    {
        Visit(node.Object);
        _currentComparison.Operation = sqlOperator;
        Visit(node.Arguments[0]);
        _currentComparison.Value = $"{prefix}{_currentComparison.Value}{suffix}";
    }

    private void HandleEqualsMethod(MethodCallExpression node)
    {
        Visit(node.Object);
        _currentComparison.Operation = "=";
        Visit(node.Arguments[0]);
    }

    private void HandleIsNullOrEmptyMethod(MethodCallExpression node)
    {
        Visit(node.Arguments[0]);
        _currentComparison.Operation = "NULL_OR_EMPTY";
    }

    private void HandleIsNullOrWhiteSpaceMethod(MethodCallExpression node)
    {
        Visit(node.Arguments[0]);
        _currentComparison.Operation = "NULL_OR_WHITESPACE";
    }

    private void HandleCollectionContainsMethod(MethodCallExpression node)
    {
        if (node.Arguments.Count == 2)
        {
            Visit(node.Arguments[1]); // Member
            _currentComparison.Operation = "IN";

            var lambda = Expression.Lambda(node.Arguments[0]);
            var value = lambda.Compile().DynamicInvoke()!;

            var formattedResult = string.Empty;

            if (value is IEnumerable enumerable)
            {
                // Convert each item in enumerable to a properly formatted string
                var items = enumerable.Cast<object>().Select(item =>
                {
                    if (item is string strItem)
                        return $"'{strItem.Replace("'", "''")}'";  // String items enclosed in single quotes
                    return item?.ToString() ?? "null";            // Non-string items are used directly
                });

                formattedResult = $"({string.Join(", ", items)})";
            }
            
            _currentComparison.Value = formattedResult;
        }
    }
}